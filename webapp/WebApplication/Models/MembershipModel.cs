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
        public bool IsScheduledSwitch { get; set; }
        public bool IsSelectable { get; set; }
        public int ActiveUserMembershipId { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.AutoRenewLabel)]
        public bool IsAutoRenew { get; set; }

        public MembershipModel(MembershipOption membershipOption)
        {
            MembershipOption = membershipOption;
        }

        public string MembershipDisplayCssClass => IsSelected ? "membership-selected" : IsUpgrade ? "membership-upgrade" : "";

        public string MembershipHoverCssClass => IsSelected ? "" : "shadow-hover";
    }
}