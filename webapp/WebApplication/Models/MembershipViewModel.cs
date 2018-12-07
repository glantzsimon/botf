using System.Collections.Generic;
using K9.DataAccessLayer.Models;

namespace K9.WebApplication.Models
{
    public class MembershipViewModel
    {
        public List<UserMembership> ExistingMemberships { get; set; }
        public List<MembershipModel> Memberships { get; set; }
    }
}