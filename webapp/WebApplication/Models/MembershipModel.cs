using K9.DataAccessLayer.Models;

namespace K9.WebApplication.Models
{
    public class MembershipModel
    {
        public MembershipOption MembershipOption { get; }
        public UserMembership UserMembership { get; }
        public bool IsSelected { get; set; }
        public bool IsSubscribed { get; set; }
        public bool IsUpgrade { get; set; }

        public int ActiveSubscriptionId => UserMembership?.MembershipOptionId ?? 0;

        public string SubscriptionStatus => UserMembership != null
            ? Globalisation.Dictionary.Subscribed
            : Globalisation.Dictionary.SubscribeNow;

        public MembershipModel(MembershipOption membershipOption, bool isSubsribed, bool isSelected, bool isUpgrade)
        {
            MembershipOption = membershipOption;
            IsSubscribed = isSubsribed;
            IsSelected = isSelected;
            IsUpgrade = isUpgrade;
        }

        public string MembershipDisplayCssClass => IsSelected ? "membership-selected" : IsUpgrade ? "membership-upgrade" : "";
    }
}