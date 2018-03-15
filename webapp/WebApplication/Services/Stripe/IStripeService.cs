using K9.WebApplication.Models;
using Stripe;

namespace K9.WebApplication.Services.Stripe
{
    public interface IStripeService
    {
        void Charge(StripeModel model);
        StripeList<StripeCharge> GetCharges();
    }
}