using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Models;
using K9.WebApplication.Services.Stripe;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using K9.SharedLibrary.Helpers;

namespace K9.WebApplication.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly ILogger _logger;
        private readonly IAuthentication _authentication;
        private readonly IRepository<MembershipOption> _membershipOptionRepository;
        private readonly IRepository<UserMembership> _userMembershipRepository;
        private readonly StripeConfiguration _stripeConfig;
        private readonly IStripeService _stripeService;
        private readonly IContactService _contactService;
        private readonly IMailer _mailer;

        public MembershipService(ILogger logger, IAuthentication authentication, IRepository<MembershipOption> membershipOptionRepository, IRepository<UserMembership> userMembershipRepository, IOptions<StripeConfiguration> stripeConfig, IStripeService stripeService, IContactService contactService, IMailer mailer)
        {
            _logger = logger;
            _authentication = authentication;
            _membershipOptionRepository = membershipOptionRepository;
            _userMembershipRepository = userMembershipRepository;
            _stripeConfig = stripeConfig.Value;
            _stripeService = stripeService;
            _contactService = contactService;
            _mailer = mailer;
        }

        public MembershipViewModel GetMembershipViewModel(int? userId = null)
        {
            userId = userId ?? _authentication.CurrentUserId;
            var membershipOptions = _membershipOptionRepository.List();
            var userMemberships = GetActiveUserMemberships();
            var primaryMembership = GetPrimaryActiveUserMembership(userId);
            return new MembershipViewModel
            {
                Memberships = new List<MembershipModel>(membershipOptions.Select(membershipOption =>
                {
                    var userMembership = userMemberships.FirstOrDefault(_ =>
                        _.UserId == userId & _.MembershipOptionId == membershipOption.Id);
                    return new MembershipModel(
                        membershipOption,
                        userMembership != null,
                        false,
                        primaryMembership != null && membershipOption.CanUpgradeFrom(primaryMembership.MembershipOption),
                        primaryMembership?.Id ?? 0
                    );
                }))
            };
        }

        public List<UserMembership> GetActiveUserMemberships(int? userId = null)
        {
            userId = userId ?? _authentication.CurrentUserId;
            var membershipOptions = _membershipOptionRepository.List();
            var userMemberships = _authentication.IsAuthenticated
                ? _userMembershipRepository.Find(_ => _.UserId == userId).ToList().Where(_ => _.IsActive).Select(userMembership =>
                {
                    userMembership.MembershipOption = membershipOptions.FirstOrDefault(m => m.Id == userMembership.MembershipOptionId);
                    return userMembership;
                }).ToList()
                : new List<UserMembership>();
            return userMemberships;
        }

        /// <summary>
        /// Sometimes user memberships can overlap, when upgrading for example. This returns the primary membership.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserMembership GetPrimaryActiveUserMembership(int? userId = null)
        {
            return GetActiveUserMemberships(userId).OrderByDescending(_ => _.MembershipOption.SubscriptionType)
                .FirstOrDefault();
        }

        public MembershipModel GetSwitchMembershipModel(int id)
        {
            var userMemberships = GetActiveUserMemberships();
            if (!userMemberships.Any())
            {
                throw new Exception(Globalisation.Dictionary.SwitchMembershipErrorNotSubscribed);
            }

            var primaryUserMembership = GetPrimaryActiveUserMembership();
            if (primaryUserMembership.MembershipOptionId == id)
            {
                throw new Exception(Globalisation.Dictionary.SwitchMembershipErrorAlreadySubscribed);
            }

            var membershipOption = _membershipOptionRepository.Find(id);
            return new MembershipModel(
                membershipOption, false, true,
                membershipOption.CanUpgradeFrom(primaryUserMembership.MembershipOption),
                primaryUserMembership?.Id ?? 0);
        }

        public MembershipModel GetPurchaseMembershipModel(int id)
        {
            var primaryUserMembership = GetPrimaryActiveUserMembership();
            if (primaryUserMembership?.MembershipOptionId == id)
            {
                throw new Exception(Globalisation.Dictionary.PurchaseMembershipErrorAlreadySubscribed);
            }

            var membershipOption = _membershipOptionRepository.Find(id);
            return new MembershipModel(
                membershipOption, false, true, false, primaryUserMembership?.Id ?? 0);
        }

        public StripeModel GetPurchaseStripeModel(int id)
        {
            var primaryUserMembership = GetPrimaryActiveUserMembership();
            var membershipOption = _membershipOptionRepository.Find(id);
            if (membershipOption == null)
            {
                _logger.Error($"MembershipService => GetPurchaseStripeModel => No MembershipOption with id {id} was found.");
                throw new IndexOutOfRangeException();
            }

            return new StripeModel
            {
                PublishableKey = _stripeConfig.PublishableKey,
                SubscriptionAmount = membershipOption.Price,
                SubscriptionDiscount = primaryUserMembership != null ? GetDiscount(primaryUserMembership, membershipOption) : 0,
                Description = membershipOption.SubscriptionTypeNameLocal,
                MembershipOptionId = id,
                LocalisedCurrencyThreeLetters = StripeModel.GetLocalisedCurrency()
            };
        }

        public void ProcessPurchase(StripeModel model)
        {
            try
            {
                var membershipOption = _membershipOptionRepository.Find(model.MembershipOptionId);
                if (membershipOption == null)
                {
                    _logger.Error($"MembershipService => ProcessPurchase => No MembershipOption with id {model.MembershipOptionId} was found.");
                    throw new IndexOutOfRangeException("Invalid MembershipOptionId");
                }

                var result = _stripeService.Charge(model);
                _userMembershipRepository.Create(new UserMembership
                {
                    UserId = _authentication.CurrentUserId,
                    MembershipOptionId = model.MembershipOptionId,
                    StartsOn = DateTime.Today,
                    EndsOn = membershipOption.IsAnnual ? DateTime.Today.AddYears(1) : DateTime.Today.AddMonths(1)
                });
                TerminateExistingMemberships(model.MembershipOptionId);
                _contactService.CreateCustomer(result.StripeCustomer.Id, model.StripeBillingName, model.StripeEmail);
            }
            catch (Exception ex)
            {
                _logger.Error($"MembershipService => ProcessPurchase => Purchase failed: {ex.Message}");
                throw ex;
            }
        }

        private void TerminateExistingMemberships(int primaryUserMembershipId)
        {
            var userMemberships = GetActiveUserMemberships();
            var primaryUserMembership =
                userMemberships.FirstOrDefault(_ => _.MembershipOptionId == primaryUserMembershipId);
            if (primaryUserMembership == null)
            {
                _logger.Error($"MembershipService => TerminateExistingMemberships => PrimaryMembership cannot be determined or does not exist");
                throw new Exception("Primary membership not found");
            }
            foreach (var userMembership in userMemberships.Where(_ => _.MembershipOptionId != primaryUserMembershipId))
            {
                userMembership.EndsOn = primaryUserMembership.StartsOn;
                _userMembershipRepository.Update(userMembership);
            }
        }

        private double GetDiscount(UserMembership userMembership, MembershipOption membershipOption)
        {
            var timeRemaining = userMembership.EndsOn.Subtract(DateTime.Today);
            var percentageRemaining = (double)timeRemaining.Ticks / (double)userMembership.Duration.Ticks;
            return userMembership.MembershipOption.Price * percentageRemaining;
        }

    }
}