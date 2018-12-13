using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Services;
using K9.WebApplication.Services.Stripe;
using Moq;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{

    public class MembershipServiceTests
    {
        private readonly Mock<IRepository<UserMembership>> _userMembershipRepository = new Mock<IRepository<UserMembership>>();
        private readonly Mock<IRepository<MembershipOption>> _membershipOptionRepository = new Mock<IRepository<MembershipOption>>();
        private readonly Mock<ILogger> _logger = new Mock<ILogger>();
        private readonly Mock<IMailer> _mailer = new Mock<IMailer>();
        private readonly Mock<IOptions<WebsiteConfiguration>> _config = new Mock<IOptions<WebsiteConfiguration>>();
        private readonly Mock<IAuthentication> _authentication = new Mock<IAuthentication>();
        private readonly Mock<IOptions<StripeConfiguration>> _stripeConfig = new Mock<IOptions<StripeConfiguration>>();
        private readonly Mock<IStripeService> _stripeService = new Mock<IStripeService>();
        private readonly Mock<IContactService> _contactService = new Mock<IContactService>();
        private MembershipService _membershipService;

        private readonly int _userId = 1;

        private readonly MembershipOption _standardMonthlyMembership = new MembershipOption
        {
            Id = 1,
            Name = "Monthly Standard Membership",
            SubscriptionType = MembershipOption.ESubscriptionType.MonthlyStandard,
            Price = 10
        };

        private readonly MembershipOption _standardYearlyMembership = new MembershipOption
        {
            Id = 2,
            Name = "Annual Standard Membership",
            SubscriptionType = MembershipOption.ESubscriptionType.AnnualStandard,
            Price = 90
        };

        private readonly MembershipOption _platinumMonthlyMembership = new MembershipOption
        {
            Id = 3,
            Name = "Monthly Platinum Membership",
            SubscriptionType = MembershipOption.ESubscriptionType.MonthlyPlatinum,
            Price = 20
        };

        private readonly MembershipOption _platinumYearlyMembership = new MembershipOption
        {
            Id = 4,
            Name = "Yearly Platinum Membership",
            SubscriptionType = MembershipOption.ESubscriptionType.AnnualPlatinum,
            Price = 180
        };

        public MembershipServiceTests()
        {
            var membershipOptions = new List<MembershipOption>
            {
                _standardMonthlyMembership,
                _standardYearlyMembership,
                _platinumMonthlyMembership,
                _platinumYearlyMembership
            };

            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(new StringWriter())
            );

            _config.SetupGet(_ => _.Value).Returns(new WebsiteConfiguration
            {
                CompanyLogoUrl = "http://local",
                CompanyName = "Glantz Consulting",
                SupportEmailAddress = "support@gc.com"
            });

            _stripeConfig.SetupGet(_ => _.Value).Returns(new StripeConfiguration
            {
                SecretKey = "sk_12348765",
                PublishableKey = "pk_09872345"
            });


            _membershipOptionRepository.Setup(_ => _.List()).Returns(membershipOptions);
            _membershipOptionRepository.Setup(_ => _.Find(_standardMonthlyMembership.Id)).Returns(_standardMonthlyMembership);
            _membershipOptionRepository.Setup(_ => _.Find(_standardYearlyMembership.Id)).Returns(_standardYearlyMembership);
            _membershipOptionRepository.Setup(_ => _.Find(_platinumMonthlyMembership.Id)).Returns(_platinumMonthlyMembership);
            _membershipOptionRepository.Setup(_ => _.Find(_platinumYearlyMembership.Id)).Returns(_platinumYearlyMembership);

            _membershipService = new MembershipService(
                _logger.Object,
                _authentication.Object,
                _membershipOptionRepository.Object,
                _userMembershipRepository.Object,
                _stripeConfig.Object,
                _stripeService.Object,
                _contactService.Object,
                _mailer.Object);
        }

        [Fact]
        public void GetMembershipModel_StandardMonthly_CanUpgradeThree()
        {
            AuthenticateUser();

            var startsOn = DateTime.Today.AddDays(-7);
            var userMemberships = new List<UserMembership>
            {
                new UserMembership
                {
                    UserId = _userId,
                    MembershipOptionId = _standardMonthlyMembership.Id,
                    MembershipOption = _standardMonthlyMembership,
                    StartsOn = startsOn,
                    EndsOn = startsOn.AddMonths(1)
                }
            };

            _userMembershipRepository.Setup(_ => _.Find(It.IsAny<System.Linq.Expressions.Expression<Func<UserMembership, bool>>>()))
                .Returns(userMemberships.AsQueryable());

            var model = _membershipService.GetMembershipViewModel();

            Assert.Equal(1, _membershipService.GetActiveUserMemberships().Count);
            Assert.Equal(userMemberships.First(), _membershipService.GetActiveUserMembership());
            Assert.Equal(0, model.Memberships.Count(_ => _.IsSelected));
            Assert.Equal(3, model.Memberships.Count(_ => _.IsUpgrade));
            Assert.Equal(1, model.Memberships.Count(_ => _.IsSubscribed));
            Assert.Equal(3, model.Memberships.Count(_ => _.IsSelectable));
        }

        [Fact]
        public void GetMembershipModel_StandardYearly_CanUpgradeTwo()
        {
            AuthenticateUser();

            var startsOn = DateTime.Today.AddDays(-7);
            var userMemberships = new List<UserMembership>
            {
                new UserMembership
                {
                    UserId = _userId,
                    MembershipOptionId = _standardYearlyMembership.Id,
                    MembershipOption = _standardYearlyMembership,
                    StartsOn = startsOn,
                    EndsOn = startsOn.AddYears(1)
                }
            };

            _userMembershipRepository.Setup(_ => _.Find(It.IsAny<System.Linq.Expressions.Expression<Func<UserMembership, bool>>>()))
                .Returns(userMemberships.AsQueryable());

            var model = _membershipService.GetMembershipViewModel();

            Assert.Equal(1, _membershipService.GetActiveUserMemberships().Count);
            Assert.Equal(userMemberships.First(), _membershipService.GetActiveUserMembership());
            Assert.Equal(0, model.Memberships.Count(_ => _.IsSelected));
            Assert.Equal(2, model.Memberships.Count(_ => _.IsUpgrade));
            Assert.Equal(1, model.Memberships.Count(_ => _.IsSubscribed));
            Assert.Equal(3, model.Memberships.Count(_ => _.IsSelectable));
            Assert.Equal(0, model.Memberships.Count(_ => _.IsScheduledSwitch));
        }

        [Fact]
        public void GetMembershipModel_PlatinumMonthly_CanUpgradeOne()
        {
            AuthenticateUser();

            var startsOn = DateTime.Today.AddDays(-7);
            var userMemberships = new List<UserMembership>
            {
                new UserMembership
                {
                    UserId = _userId,
                    MembershipOptionId = _platinumMonthlyMembership.Id,
                    MembershipOption = _platinumMonthlyMembership,
                    StartsOn = startsOn,
                    EndsOn = startsOn.AddMonths(1)
                }
            };

            _userMembershipRepository.Setup(_ => _.Find(It.IsAny<System.Linq.Expressions.Expression<Func<UserMembership, bool>>>()))
                .Returns(userMemberships.AsQueryable());

            var model = _membershipService.GetMembershipViewModel();

            Assert.Equal(1, _membershipService.GetActiveUserMemberships().Count);
            Assert.Equal(userMemberships.First(), _membershipService.GetActiveUserMembership());
            Assert.Equal(0, model.Memberships.Count(_ => _.IsSelected));
            Assert.Equal(1, model.Memberships.Count(_ => _.IsUpgrade));
            Assert.Equal(1, model.Memberships.Count(_ => _.IsSubscribed));
            Assert.Equal(3, model.Memberships.Count(_ => _.IsSelectable));
            Assert.Equal(0, model.Memberships.Count(_ => _.IsScheduledSwitch));
        }

        [Fact]
        public void GetMembershipModel_PlatinumYearly_CanUpgradeNone()
        {
            AuthenticateUser();

            var startsOn = DateTime.Today.AddDays(-7);
            var userMemberships = new List<UserMembership>
            {
                new UserMembership
                {
                    Id = 7,
                    UserId = _userId,
                    MembershipOptionId = _platinumYearlyMembership.Id,
                    MembershipOption = _platinumYearlyMembership,
                    StartsOn = startsOn,
                    EndsOn = startsOn.AddYears(1),
                }
            };

            _userMembershipRepository.Setup(_ => _.Find(It.IsAny<System.Linq.Expressions.Expression<Func<UserMembership, bool>>>()))
                .Returns(userMemberships.AsQueryable());

            var model = _membershipService.GetMembershipViewModel();

            Assert.Equal(1, _membershipService.GetActiveUserMemberships().Count);
            Assert.Equal(userMemberships.First(), _membershipService.GetActiveUserMembership());
            Assert.Equal(0, model.Memberships.Count(_ => _.IsSelected));
            Assert.Equal(0, model.Memberships.Count(_ => _.IsUpgrade));
            Assert.Equal(1, model.Memberships.Count(_ => _.IsSubscribed));
            Assert.Equal(3, model.Memberships.Count(_ => _.IsSelectable));
            Assert.Equal(0, model.Memberships.Count(_ => _.IsScheduledSwitch));
            Assert.Equal(7, model.Memberships.First().ActiveUserMembershipId);
        }

        [Fact]
        public void GetMembershipModel_CanUpgradeOne_AndSwitchOne()
        {
            AuthenticateUser();

            var startsOn = DateTime.Today.AddDays(-7);
            var scheduledStartsOn = startsOn.AddMonths(1).AddDays(1);
            var scheduledUserMembership = new UserMembership
            {
                Id = 8,
                UserId = _userId,
                MembershipOptionId = _standardMonthlyMembership.Id,
                MembershipOption = _standardMonthlyMembership,
                StartsOn = scheduledStartsOn,
                EndsOn = scheduledStartsOn.AddMonths(1),
                IsAutoRenew = true
            };
            var userMemberships = new List<UserMembership>
            {
                new UserMembership
                {
                    Id = 7,
                    UserId = _userId,
                    MembershipOptionId = _platinumMonthlyMembership.Id,
                    MembershipOption = _platinumMonthlyMembership,
                    StartsOn = startsOn,
                    EndsOn = startsOn.AddMonths(1)
                },
                scheduledUserMembership
            };

            _userMembershipRepository.Setup(_ => _.Find(It.IsAny<System.Linq.Expressions.Expression<Func<UserMembership, bool>>>()))
                .Returns(userMemberships.AsQueryable());

            var model = _membershipService.GetMembershipViewModel();
            var scheduledmembershipResult = _membershipService.GetScheduledSwitchUserMembership();

            Assert.Equal(1, _membershipService.GetActiveUserMemberships().Count);
            Assert.Equal(userMemberships.First(), _membershipService.GetActiveUserMembership());
            Assert.Equal(0, model.Memberships.Count(_ => _.IsSelected));
            Assert.Equal(1, model.Memberships.Count(_ => _.IsUpgrade));
            Assert.Equal(2, model.Memberships.Count(_ => _.IsSubscribed));
            Assert.Equal(2, model.Memberships.Count(_ => _.IsSelectable));
            Assert.Equal(1, model.Memberships.Count(_ => _.IsScheduledSwitch));
            Assert.Equal(7, model.Memberships.First().ActiveUserMembershipId);
            Assert.Equal(scheduledUserMembership, scheduledmembershipResult);
        }

        [Fact]
        public void GetSwitchMembershipModel_ShouldThrowError_NoSubscriptions()
        {
            AuthenticateUser();

            var ex = Assert.Throws<Exception>(() => _membershipService.GetSwitchMembershipModel(1));
            Assert.Equal(Globalisation.Dictionary.SwitchMembershipErrorNotSubscribed, ex.Message);
        }

        [Fact]
        public void GetSwitchMembershipModel_ShouldThrowError_SameSubscriptionId()
        {
            AuthenticateUser();

            var startsOn = DateTime.Today.AddDays(-7);
            var userMemberships = new List<UserMembership>
            {
                new UserMembership
                {
                    Id = 7,
                    UserId = _userId,
                    MembershipOptionId = _standardMonthlyMembership.Id,
                    MembershipOption = _standardMonthlyMembership,
                    StartsOn = startsOn,
                    EndsOn = startsOn.AddMonths(1)
                }
            };

            _userMembershipRepository.Setup(_ => _.Find(It.IsAny<System.Linq.Expressions.Expression<Func<UserMembership, bool>>>()))
                .Returns(userMemberships.AsQueryable());


            var ex = Assert.Throws<Exception>(() => _membershipService.GetSwitchMembershipModel(_standardMonthlyMembership.Id));
            Assert.Equal(Globalisation.Dictionary.SwitchMembershipErrorAlreadySubscribed, ex.Message);
        }

        [Fact]
        public void GetSwitchMembershipModel_ShouldThrowError_ScheduledIsSameSubscriptionId()
        {
            AuthenticateUser();

            var startsOn = DateTime.Today.AddDays(-7);
            var scheduledStartsOn = startsOn.AddMonths(1).AddDays(1);
            var scheduledUserMembership = new UserMembership
            {
                Id = 8,
                UserId = _userId,
                MembershipOptionId = _standardYearlyMembership.Id,
                MembershipOption = _standardYearlyMembership,
                StartsOn = scheduledStartsOn,
                EndsOn = scheduledStartsOn.AddMonths(1),
                IsAutoRenew = true
            };
            var userMemberships = new List<UserMembership>
            {
                new UserMembership
                {
                    Id = 7,
                    UserId = _userId,
                    MembershipOptionId = _platinumMonthlyMembership.Id,
                    MembershipOption = _platinumMonthlyMembership,
                    StartsOn = startsOn,
                    EndsOn = startsOn.AddMonths(1)
                },
                scheduledUserMembership
            };

            _userMembershipRepository.Setup(_ => _.Find(It.IsAny<System.Linq.Expressions.Expression<Func<UserMembership, bool>>>()))
                .Returns(userMemberships.AsQueryable());


            var ex = Assert.Throws<Exception>(() => _membershipService.GetSwitchMembershipModel(_standardYearlyMembership.Id));
            Assert.Equal(Globalisation.Dictionary.SwitchMembershipErrorAlreadySubscribed, ex.Message);
        }

        [Fact]
        public void GetSwitchMembershipModel_isUpgrade()
        {
            AuthenticateUser();

            var startsOn = DateTime.Today.AddDays(-7);
            var userMemberships = new List<UserMembership>
            {
                new UserMembership
                {
                    Id = 7,
                    UserId = _userId,
                    MembershipOptionId = _standardMonthlyMembership.Id,
                    MembershipOption = _standardMonthlyMembership,
                    StartsOn = startsOn,
                    EndsOn = startsOn.AddMonths(1)
                }
            };

            _userMembershipRepository.Setup(_ => _.Find(It.IsAny<System.Linq.Expressions.Expression<Func<UserMembership, bool>>>()))
                .Returns(userMemberships.AsQueryable());


            var result = _membershipService.GetSwitchMembershipModel(_standardYearlyMembership.Id);
            Assert.True(result.IsUpgrade);
        }

        private void AuthenticateUser()
        {
            _authentication.SetupGet(_ => _.IsAuthenticated).Returns(true);
            _authentication.SetupGet(_ => _.CurrentUserId).Returns(_userId);
        }
    }
}
