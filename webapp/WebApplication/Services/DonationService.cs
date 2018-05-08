using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Services.Stripe;
using NLog;
using System;
using System.Web;
using System.Web.Mvc;

namespace K9.WebApplication.Services
{
    public class DonationService : IDonationService
    {
        private readonly IRepository<Donation> _donationRepository;
        private readonly ILogger _logger;
        private readonly IMailer _mailer;
        private readonly IStripeService _stripeService;
        private readonly WebsiteConfiguration _config;
        private readonly UrlHelper _urlHelper;

        public DonationService(IRepository<Donation> donationRepository, ILogger logger, IMailer mailer, IOptions<WebsiteConfiguration> config, IStripeService stripeService)
        {
            _donationRepository = donationRepository;
            _logger = logger;
            _mailer = mailer;
            _stripeService = stripeService;
            _config = config.Value;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public void CreateDonation(Donation donation)
        {
            try
            {
                _donationRepository.Create(donation);
                var template = donation.NumberOfIbogas > 0
                    ? Globalisation.Dictionary.IbogaSponsoredEmail
                    : Globalisation.Dictionary.DonationReceivedEmail;
                var title = donation.NumberOfIbogas > 0 ? "We have received a donation to sponsor an iboga tree!" : "We have received a donation!"; 
                _mailer.SendEmail("New Donation", TemplateProcessor.PopulateTemplate(template, new
                {
                    Title = title,
                    donation.Customer,
                    donation.CustomerEmail,
                    Amount = donation.DonationAmount,
                    donation.Currency,
                    LinkToSummary = _urlHelper.AsboluteAction("Index", "Donations"),
                    Company = _config.CompanyName,
                    ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl),
                    donation.NumberOfIbogas
                }), _config.SupportEmailAddress, _config.CompanyName, _config.SupportEmailAddress, _config.CompanyName);
            }
            catch (Exception ex)
            {
                _logger.Error($"DonationService => CreateDonation => {ex.Message}");
            }
        }
    }
}