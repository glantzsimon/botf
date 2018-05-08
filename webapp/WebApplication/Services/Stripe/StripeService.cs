using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Models;
using Stripe;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.Services.Stripe
{
    public class StripeService : IStripeService
    {
        private readonly Config.StripeConfiguration _stripeConfig;

        public StripeService(IOptions<Config.StripeConfiguration> stripeConfig)
        {
            _stripeConfig = stripeConfig.Value;
        }

        public string Charge(StripeModel model)
        {
            var customers = new StripeCustomerService();
            var charges = new StripeChargeService();

            var customer = customers.Create(new StripeCustomerCreateOptions
            {
                Email = model.StripeEmail,
                SourceToken = model.StripeToken,
                Description = model.Description
            });

            var charge = charges.Create(new StripeChargeCreateOptions
            {
                Amount = (int)model.DonationAmountInCents,
                Description = model.Description,
                Currency = model.LocalisedCurrencyThreeLetters,
                CustomerId = customer.Id
            });

            return charge.Id;
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

        public List<Donation>  GetDonations()
        {
            return GetCharges().Select(c =>
                new Donation
                {
                    StripeId = c.Id,
                    Customer = c.Customer?.Description ?? c.Source?.Card?.Name,
                    Currency = c.Currency.ToUpper(),
                    CustomerEmail = c.Customer?.Email,
                    DonationDescription = c.Description + (c.Refunded ? " (refunded)" : ""),
                    DonatedOn = c.Created,
                    DonationAmount = (c.Amount / 100) * (c.Refunded ? -1 : 1)
                }).ToList();
        }
    }
}