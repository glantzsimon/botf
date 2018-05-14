using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using NLog;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace K9.WebApplication.Services
{
    public class DonationService : IDonationService
    {
        private readonly IRepository<Donation> _donationRepository;
        private readonly ILogger _logger;
        private readonly IMailer _mailer;
        private readonly WebsiteConfiguration _config;
        private readonly UrlHelper _urlHelper;

        public DonationService(IRepository<Donation> donationRepository, ILogger logger, IMailer mailer, IOptions<WebsiteConfiguration> config)
        {
            _donationRepository = donationRepository;
            _logger = logger;
            _mailer = mailer;
            _config = config.Value;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public void CreateDonation(Donation donation)
        {
            try
            {
                _donationRepository.Create(donation);
                SendEmailToBotf(donation);
                SendEmailToCustomer(donation);
            }
            catch (Exception ex)
            {
                _logger.Error($"DonationService => CreateDonation => {ex.Message}");
            }
        }

        public int GetNumberOfIbogasSponsoredToDate()
        {
            return _donationRepository.List().Sum(d => d.NumberOfIbogas);
        }

        public int GetNumberOfIbogasSponsoredLast30Days()
        {
            return _donationRepository.List().Where(d => DateTime.Today.Subtract(d.DonatedOn).TotalDays <= 30).Sum(d => d.NumberOfIbogas);
        }

        public int GetProjectedNumberOfIbogasSponsoredPerYear()
        {
            return GetNumberOfIbogasSponsoredLast30Days() * 12;
        }

        private void SendEmailToBotf(Donation donation)
        {
            var template = donation.NumberOfIbogas > 0
                ? Dictionary.IbogaSponsoredEmail
                : Dictionary.DonationReceivedEmail;
            var title = donation.NumberOfIbogas > 0
                ? "We have received a donation to sponsor an iboga tree!"
                : "We have received a donation!";
            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
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

        private void SendEmailToCustomer(Donation donation)
        {
            var template = donation.NumberOfIbogas > 0
                ? Dictionary.SponsorThankYouEmail
                : Dictionary.DonationThankYouEmail;
            var title = donation.NumberOfIbogas > 0
                ? Dictionary.ThankyouForSponorEmailTitle
                : Dictionary.ThankyouForDonationEmailTitle;
            _mailer.SendEmail(title, TemplateProcessor.PopulateTemplate(template, new
            {
                Title = title,
                donation.CustomerName,
                donation.CustomerEmail,
                Amount = donation.DonationAmount,
                donation.Currency,
                Company = _config.CompanyName,
                ImageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl),
                donation.NumberOfIbogas
            }), donation.CustomerEmail, donation.Customer, _config.SupportEmailAddress, _config.CompanyName);
        }
    }
}