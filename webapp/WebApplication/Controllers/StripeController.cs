using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.UnitsOfWork;
using K9.WebApplication.Services.Stripe;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [Route("donations/list")]
    public class StripeController : BaseController<Charge>
    {
        private readonly IStripeService _stripeService;

        public StripeController(IControllerPackage<Charge> controllerPackage, IStripeService stripeService)
            : base(controllerPackage)
        {
            _stripeService = stripeService;
        }
    }
}