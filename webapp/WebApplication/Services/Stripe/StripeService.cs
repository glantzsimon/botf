using K9.SharedLibrary.Models;
using K9.WebApplication.Models;
using Stripe;

namespace K9.WebApplication.Services.Stripe
{
    public class StripeService : IStripeService
    {
        private readonly Config.StripeConfiguration _stripeConfig;

        public StripeService(IOptions<Config.StripeConfiguration> stripeConfig)
        {
            _stripeConfig = stripeConfig.Value;
        }

        public void Charge(StripeModel model)
        {
            var customers = new StripeCustomerService();
            var charges = new StripeChargeService();

            var customer = customers.Create(new StripeCustomerCreateOptions
            {
                Email = model.StripeEmail,
                SourceToken = model.StripeToken,
                Description = model.Description
            });

            charges.Create(new StripeChargeCreateOptions
            {
                Amount = (int)model.DonationAmountInCents,
                Description = model.Description,
                Currency = model.LocalisedCurrencyThreeLetters,
                CustomerId = customer.Id
            });
        }

        public StripeList<StripeCharge> GetCharges()
        {
            StripeConfiguration.SetApiKey(_stripeConfig.SecretKey);

            var chargeService = new StripeChargeService();
            return chargeService.List(
                new StripeChargeListOptions()
                {
                    Limit = 30
                }
            );
        }
    }
}