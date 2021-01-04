using System.IO;
using System.Web;
using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Controllers;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using K9.WebApplication.Services.Stripe;
using Moq;
using NLog;
using Stripe;
using Xunit;
using StripeConfiguration = K9.WebApplication.Config.StripeConfiguration;

namespace K9.WebApplication.Tests.Unit.Controllers
{

    public class SupportControllerTests
    {
        private readonly Mock<IRoles> _roles = new Mock<IRoles>();
        private readonly Mock<IStripeService> _stripeService = new Mock<IStripeService>();
        private readonly Mock<IFileSourceHelper> _fileSourceHelper = new Mock<IFileSourceHelper>();
        private readonly Mock<IAuthentication> _authentication = new Mock<IAuthentication>();
        private readonly Mock<IDataSetsHelper> _datasetHelper = new Mock<IDataSetsHelper>();
        private readonly Mock<IDonationService> _donationService = new Mock<IDonationService>();
        private readonly Mock<IContactService> _contactService = new Mock<IContactService>();
        private readonly Mock<ILogger> _logger = new Mock<ILogger>();
        private readonly Mock<IMailer> _mailer = new Mock<IMailer>();
        private readonly Mock<IOptions<WebsiteConfiguration>> _config = new Mock<IOptions<WebsiteConfiguration>>();
        private readonly Mock<IOptions<StripeConfiguration>> _stripeConfig = new Mock<IOptions<StripeConfiguration>>();
        private readonly Mock<IRepository<User>> _userRepository = new Mock<IRepository<User>>();
        private readonly Mock<IMailChimpService> _mailChimpService = new Mock<IMailChimpService>();
        private SupportController _supportController;

        public SupportControllerTests()
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(new StringWriter())
            );

            _supportController = new SupportController(
                _logger.Object,
                _datasetHelper.Object,
                _roles.Object,
                _mailer.Object,
                _config.Object,
                _authentication.Object,
                _fileSourceHelper.Object,
                _stripeService.Object,
                _stripeConfig.Object,
                _donationService.Object,
                _userRepository.Object,
                _contactService.Object,
                _mailChimpService.Object);
        }

       
    }
}
