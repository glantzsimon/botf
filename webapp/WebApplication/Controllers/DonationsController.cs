using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.WebApplication.Services;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    public class DonationsController : BaseController<Donation>
    {
        private readonly IDonationService _donationService;

        public DonationsController(IControllerPackage<Donation> controllerPackage, IDonationService donationService)
            : base(controllerPackage)
        {
            _donationService = donationService;
        }

        public ActionResult LiveFeed()
        {
            return View(_donationService.GetLiveFeed());
        }
    }
}