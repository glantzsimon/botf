using K9.DataAccessLayer.Models;
using Stripe;

namespace K9.WebApplication.Services
{
    public interface IContactService
    {
        void CreateCustomer(string stripeCustomerId, string fullName, string emailAddress);
    }
}