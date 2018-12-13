using K9.DataAccessLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.Models
{
    public class MembershipModel
    {
        public MembershipOption MembershipOption { get; }
        public bool IsSelected { get; set; }
        public bool IsSubscribed { get; set; }
        public bool IsUpgrade { get; set; }
        public bool IsScheduledDowngrade { get; set; }
        public bool IsSelectable { get; set; }
        public int ActiveMembershipId { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.AutoRenewLabel)]
        public bool IsAutoRenew { get; set; }

        public MembershipModel(MembershipOption membershipOption, bool isSubsribed, bool isSelected, bool isUpgrade, bool isScheduledDowngrade, bool isSelectable, int activeMembershipId)
        {
            MembershipOption = membershipOption;
            IsSubscribed = isSubsribed;
            IsSelected = isSelected;
            IsUpgrade = isUpgrade;
            IsScheduledDowngrade = isScheduledDowngrade;
            IsSelectable = isSelectable;
            ActiveMembershipId = activeMembershipId;
        }

        public string MembershipDisplayCssClass => IsSelected ? "membership-selected" : IsUpgrade ? "membership-upgrade" : "";

        public string MembershipHoverCssClass => IsSelected ? "" : "shadow-hover";
    }
}