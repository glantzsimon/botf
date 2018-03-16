using K9.DataAccessLayer.Models;
using K9.WebApplication.Models;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public interface IDonationService
    {
        void CreateDonation(Donation donation);
        List<DonationItem> GetLiveFeed();
    }
}