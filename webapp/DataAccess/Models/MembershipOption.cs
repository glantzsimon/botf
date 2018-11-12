﻿using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Authentication;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading;
using K9.Base.DataAccessLayer.Extensions;

namespace K9.DataAccessLayer.Models
{
    [Grammar(ResourceType = typeof(Dictionary), DefiniteArticleName = Base.Globalisation.Strings.Grammar.DefiniteArticleWithApostrophe, IndefiniteArticleName = Base.Globalisation.Strings.Grammar.FeminineIndefiniteArticle)]
    [Name(ResourceType = typeof(Dictionary), ListName = Strings.Names.MembershipOptions, PluralName = Strings.Names.MembershipOptions, Name = Strings.Names.Donation)]
    [Description(UseLocalisedString = true, ResourceType = typeof(Dictionary))]
    [DefaultPermissions(Role = RoleNames.PowerUsers)]
    public class MembershipOption : ObjectBase
    {
        public enum ESubscriptionType
        {
            [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.MonthlyStandardMembership)]
            MonthlyStandard,
            [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.AnnualStandardMembership)]
            AnnualStandard,
            [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.MonthlyPlatinumMembership)]
            MonthlyPlatinum,
            [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.AnnualPlatinumMembership)]
            AnnualPlatinum
        }

        [Required]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.MembershipLabel)]
        public ESubscriptionType SubscriptionType { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.MembershipLabel)]
        public string SubscriptionTypeName => SubscriptionType.GetLocalisedLanguageName();

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SubscriptionDetailsLabel)]
        [Required(ErrorMessageResourceType = typeof(Base.Globalisation.Dictionary), ErrorMessageResourceName = Base.Globalisation.Strings.ErrorMessages.FieldIsRequired)]
        public string SubscriptionDetails { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SubscriptionDetailsLabel)]
        public string SubscriptionDetailsLocal => GetLocalisedPropertyValue(nameof(SubscriptionDetails));

        [Required]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SubscriptionCostLabel)]
        [DataType(DataType.Currency)]
        public double Price { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SubscriptionCostLabel)]
        public string FormattedPrice => Price.ToString("C0", Thread.CurrentThread.CurrentUICulture);

        public string CssClassName => SubscriptionType == ESubscriptionType.AnnualPlatinum ||
                                      SubscriptionType == ESubscriptionType.MonthlyPlatinum
            ? "platinum"
            : "standard";

        public string MembershipMedalElement => SubscriptionType == ESubscriptionType.AnnualPlatinum ||
                                              SubscriptionType == ESubscriptionType.MonthlyPlatinum
            ? "PlatinumMembership"
            : "StandardMembership";

        public string MembershipMedalElementLocal => GetLocalisedPropertyValue(nameof(MembershipMedalElement));

        public string MembershipPeriod => SubscriptionType == ESubscriptionType.AnnualPlatinum ||
                                                SubscriptionType == ESubscriptionType.AnnualStandard
            ? "Annual"
            : "Monthly";

        public string MembershipPeriodLocal => GetLocalisedPropertyValue(nameof(MembershipPeriod));
    }
}