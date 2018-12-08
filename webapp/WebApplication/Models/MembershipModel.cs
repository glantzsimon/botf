using K9.DataAccessLayer.Models;

namespace K9.WebApplication.Models
{
    public class MembershipModel
    {
        public MembershipOption MembershipOption { get; }
        public bool IsSelected { get; set; }
        public bool IsSubscribed { get; set; }
        public bool IsUpgrade { get; set; }
        public int PrimaryMembershipId { get; set; }
      
        public MembershipModel(MembershipOption membershipOption, bool isSubsribed, bool isSelected, bool isUpgrade, int primaryMembershipId)
        {
            MembershipOption = membershipOption;
            IsSubscribed = isSubsribed;
            IsSelected = isSelected;
            IsUpgrade = isUpgrade;
            PrimaryMembershipId = primaryMembershipId;
        }

        public string MembershipDisplayCssClass => IsSelected ? "membership-selected" : IsUpgrade ? "membership-upgrade" : "";
    }
}