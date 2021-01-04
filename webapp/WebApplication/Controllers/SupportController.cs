using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Config;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.ViewModels;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using K9.WebApplication.Services.Stripe;
using NLog;
using System;
using System.Threading;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class SupportController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IMailer _mailer;
        private readonly IAuthentication _authentication;
        private readonly IStripeService _stripeService;
        private readonly IDonationService _donationService;
        private readonly IRepository<User> _userRepository;
        private readonly IContactService _contactService;
        private readonly IMailChimpService _mailChimpService;
        private readonly StripeConfiguration _stripeConfig;
        private readonly WebsiteConfiguration _config;

        public SupportController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IMailer mailer, IOptions<WebsiteConfiguration> config, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IStripeService stripeService, IOptions<StripeConfiguration> stripeConfig, IDonationService donationService, IRepository<User> userRepository, IContactService contactService, IMailChimpService mailChimpService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
        {
            _logger = logger;
            _mailer = mailer;
            _authentication = authentication;
            _stripeService = stripeService;
            _donationService = donationService;
            _userRepository = userRepository;
            _contactService = contactService;
            _mailChimpService = mailChimpService;
            _stripeConfig = stripeConfig.Value;
            _config = config.Value;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactUs(ContactUsViewModel model)
        {
            try
            {
                _mailer.SendEmail(
                    model.Subject,
                    model.Body,
                    _config.SupportEmailAddress,
                    _config.CompanyName,
                    model.EmailAddress,
                    model.Name);

                return RedirectToAction("ContactUsSuccess");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetFullErrorMessage());
                return View("FriendlyError");
            }
        }

        public ActionResult ContactUsSuccess()
        {
            return View();
        }

        [Route("donate/start")]
        public ActionResult DonateStart()
        {
            return View(new Donation
            {
                DonationAmount = 10,
                DonationDescription = Dictionary.DonationToBOTF
            });
        }

        [Route("sponsor-iboga/start")]
        public ActionResult SponsorIbogaStart()
        {
            return View(new Donation
            {
                NumberOfIbogas = 1,
                DonationDescription = Dictionary.SponsorIbogaTree
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Donate(Donation donation)
        {
            ViewBag.PublishableKey = _stripeConfig.PublishableKey;
            return View(donation);
        }

        [Route("sponsor-iboga")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SponsorIboga(Donation donation)
        {
            if (donation.NumberOfIbogas < 1)
            {
                // Silly, but you never know - may as well procedd with minimum one
                donation.NumberOfIbogas = 1;
            }
            return View(donation);
        }

        [Route("donate/success")]
        public ActionResult DonationSuccess()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ProcessDonation(PaymentModel paymentModel)
        {
            try
            {
                var contact = _contactService.Find(paymentModel.ContactId);
                _mailChimpService.AddContact(contact.Name, contact.EmailAddress);

                _donationService.CreateDonation(new Donation
                {
                    Currency = paymentModel.Currency,
                    Customer = paymentModel.CustomerName,
                    CustomerEmail = paymentModel.CustomerEmailAddress,
                    DonationDescription = paymentModel.Description,
                    DonatedOn = DateTime.Now,
                    DonationAmount = paymentModel.Amount,
                    Status = paymentModel.Status
                }, contact);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.Error($"SupportController => ProcessDonation => Error: {ex.GetFullErrorMessage()}");
                return Json(new { success = false, error = ex.Message });
            }
        }

        [Route("sponsor-iboga/success")]
        public ActionResult SponsorSuccess()
        {
            return View();
        }
        
        [HttpPost]
          public ActionResult ProcessSponsor(PaymentModel paymentModel)
        {
            try
            {
                var contact = _contactService.Find(paymentModel.ContactId);
                _mailChimpService.AddContact(contact.Name, contact.EmailAddress);
                paymentModel.Description = Dictionary.SponsorIbogaTree;
                
                _donationService.CreateDonation(new Donation
                {
                    Currency = paymentModel.Currency,
                    Customer = paymentModel.CustomerName,
                    CustomerEmail = paymentModel.CustomerEmailAddress,
                    DonationDescription = paymentModel.Description,
                    DonatedOn = DateTime.Now,
                    DonationAmount = paymentModel.Amount,
                    NumberOfIbogas = paymentModel.Quantity,
                    Status = paymentModel.Status
                }, contact);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.Error($"SupportController => Sponsor => Sponsor failed: {ex.Message}");
                return Json(new { success = false, error = ex.Message });
            }
        }

        public override string GetObjectName()
        {
            throw new NotImplementedException();
        }
    }
}
