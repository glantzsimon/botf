using System.IO;
using System.Web;
using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Services;
using Moq;
using NLog;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{

    public class DonationServiceTests
    {
        private readonly Mock<IRepository<Donation>> _donationRepository = new Mock<IRepository<Donation>>();
        private readonly Mock<ILogger> _logger = new Mock<ILogger>();
        private readonly Mock<IMailer> _mailer = new Mock<IMailer>();
        private readonly Mock<IOptions<WebsiteConfiguration>> _config = new Mock<IOptions<WebsiteConfiguration>>();
        private DonationService _donationService;

        public DonationServiceTests()
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(new StringWriter())
            );

            _config.SetupGet(_ => _.Value).Returns(new WebsiteConfiguration
            {
                CompanyLogoUrl = "http://local",
                CompanyName = "Glantz Consulting",
                SupportEmailAddress = "support@gc.com"
            });

            _donationService = new DonationService(_donationRepository.Object, _logger.Object, _mailer.Object,
                _config.Object);
        }

        [Fact]
        public void CreateDonation_CreatesDonationAndSendsTwoEmails()
        {
            Donation expectedDonation = null;

            _donationRepository.Setup(_ => _.Create(It.IsAny<Donation>()))
                .Callback<Donation>(result => expectedDonation = result);

            var actualDonation = new Donation
            {
                Customer = "Simon Glantz",
                CustomerEmail = "simon.glantz@mac.com",
                DonationAmount = 50,
                Currency = "USD",
                NumberOfIbogas = 1

            };
            _donationService.CreateDonation(actualDonation, new Contact());

            _donationRepository.Verify(_ => _.Create(It.IsAny<Donation>()), Times.Once);
            _mailer.Verify(_ => _.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Exactly(2));
            Assert.Equal(expectedDonation, actualDonation);
        }
    }
}
