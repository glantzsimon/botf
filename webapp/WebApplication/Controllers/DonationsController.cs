﻿using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.WebApplication.Services.Stripe;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    public class DonationsController : BaseController<Donation>
    {
        private readonly IStripeService _stripeService;

        public DonationsController(IControllerPackage<Donation> controllerPackage, IStripeService stripeService)
            : base(controllerPackage)
        {
            _stripeService = stripeService;
        }
    }
}