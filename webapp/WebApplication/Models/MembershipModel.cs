using K9.DataAccessLayer.Models;

namespace K9.WebApplication.Models
{
    public class MembershipModel
    {
        public MembershipOption MembershipOption { get; }
        public UserMembership UserMembership { get; }

        public int ActiveSubscriptionId => UserMembership?.MembershipOptionId ?? 0;

        public string SubscriptionStatus => UserMembership != null
            ? Globalisation.Dictionary.Subscribed
            : Globalisation.Dictionary.SubscribeNow;

        public MembershipModel(MembershipOption membershipOption, UserMembership userMembership)
        {
            MembershipOption = membershipOption;
            UserMembership = userMembership;
        }
    }
}