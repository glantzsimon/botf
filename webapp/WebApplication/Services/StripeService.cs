using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Models;
using NLog;
using Stripe;
using System;

namespace K9.WebApplication.Services
{
    public class StripeService : IStripeService
    {
        private readonly IRepository<Donation> _donationRepository;
        private readonly ILogger _logger;

        public StripeService(IRepository<Donation> donationRepository, ILogger logger)
        {
            _donationRepository = donationRepository;
            _logger = logger;
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
                Currency = model.LocalisedCurrencySymbol,
                CustomerId = customer.Id
            });

            try
            {
                CreateDonationRecord(model);
            }
            catch (Exception ex)
            {
                _logger.Error($"StripService => Charge => {ex.Message}");
            }
        }

        public void CreateDonationRecord(StripeModel model)
        {
            _donationRepository.Create(new Donation
            {
                Currency = model.LocalisedCurrencySymbol,
                Customer = model.StripeBillingName,
                CustomerEmail = model.StripeEmail,
                DonationDescription = model.Description,
                DonatedOn = DateTime.Now,
                DonationAmount = model.AmountToDonate
            });
        }
    }
}