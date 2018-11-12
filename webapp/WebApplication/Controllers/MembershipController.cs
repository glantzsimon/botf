using K9.Base.WebApplication.Controllers;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using K9.WebApplication.Services.Stripe;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class MembershipController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IRepository<MembershipOption> _membershipOptionRepository;
        private readonly IRepository<UserMembership> _userMembershipRepository;
        private readonly IStripeService _stripeService;
        private readonly IContactService _contactService;
        private readonly StripeConfiguration _stripeConfig;

        public MembershipController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IRepository<MembershipOption> membershipOptionRepository, IRepository<UserMembership> userMembershipRepository, IOptions<StripeConfiguration> stripeConfig, IStripeService stripeService, IContactService contactService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
        {
            _logger = logger;
            _membershipOptionRepository = membershipOptionRepository;
            _userMembershipRepository = userMembershipRepository;
            _stripeService = stripeService;
            _contactService = contactService;
            _stripeConfig = stripeConfig.Value;
        }

        public ActionResult Index()
        {
            var membershipOptions = _membershipOptionRepository.List();
            var userMemberships = Authentication.IsAuthenticated
                ? _userMembershipRepository.Find(_ => _.UserId == Authentication.CurrentUserId).ToList()
                : new List<UserMembership>();
            var model = new MembershipViewModel{Memberships  = new List<MembershipModel>(membershipOptions.Select(membershipOption =>
                {
                    return new MembershipModel(membershipOption,
                        userMemberships.FirstOrDefault(_ => _.UserId == Authentication.CurrentUserId));
                }))};
            
            return View(model);
        }

        [Authorize]
        [Route("join")]
        public ActionResult PurchaseStart(int id)
        {
            return View(new MembershipModel(
                _membershipOptionRepository.Find(id),
                new UserMembership
                {
                    MembershipOptionId = id
                }));
        }

        [Route("join/purchase")]
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Purchase(int id)
        {
            var membershipOption = _membershipOptionRepository.Find(id);
            if (membershipOption == null)
            {
                _logger.Error($"MembershipController => Purchse => No MembershipOption with id {id} was found.");
                throw new IndexOutOfRangeException();
            }
            var stripeModel = new StripeModel
            {
                PublishableKey = _stripeConfig.PublishableKey,
                SubscriptionAmount = membershipOption.Price,
                Description = membershipOption.SubscriptionTypeNameLocal,
                MembershipOptionId = id,
                LocalisedCurrencyThreeLetters = StripeModel.GetLocalisedCurrency()
            };

            _logger.Debug($"MembershipController => Purchase => Current UI Thread: {Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName}");
            return View(stripeModel);
        }

        [Authorize]
        [HttpPost]
        [Route("join/processing")]
        [ValidateAntiForgeryToken]
        public ActionResult PurchaseProcess(StripeModel model)
        {
            try
            {
                var result = _stripeService.Charge(model);
                _userMembershipRepository.Create(new UserMembership
                {
                    UserId = Authentication.CurrentUserId,
                    MembershipOptionId = model.MembershipOptionId
                });
                _contactService.CreateCustomer(result.StripeCustomer.Id, model.StripeBillingName, model.StripeEmail);
                return RedirectToAction("PurchaseSuccess");
            }
            catch (Exception ex)
            {
                _logger.Error($"MembershipController => Purchase => Purchase failed: {ex.Message}");
                ModelState.AddModelError("", ex.Message);
            }

            return View("Purchase", model);
        }

        [Route("join/success")]
        public ActionResult PurchaseSuccess()
        {
            return View();
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }

    }
}
