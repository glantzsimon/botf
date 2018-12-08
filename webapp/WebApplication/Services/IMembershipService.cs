using System.Collections.Generic;
using K9.DataAccessLayer.Models;
using K9.WebApplication.Models;

namespace K9.WebApplication.Services
{
    public interface IMembershipService
    {
        MembershipViewModel GetMembershipViewModel(int? userId = null);
        MembershipModel GetSwitchMembershipModel(int id);
        MembershipModel GetPurchaseMembershipModel(int id);
        StripeModel GetPurchaseStripeModel(int id);
        void ProcessPurchase(StripeModel model);
        List<UserMembership> GetActiveUserMemberships(int? userId = null);
        UserMembership GetPrimaryActiveUserMembership(int? userId = null);
    }
}