﻿using System.Web.Mvc;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.WebApplication.Filters;
using K9.WebApplication.UnitsOfWork;

namespace K9.WebApplication.Controllers
{
	[Authorize]
	[RequirePermissions(Role = RoleNames.Administrators)]
	public class PermissionsController : BaseController<Permission>
	{
		public PermissionsController(IControllerPackage<Permission> controllerPackage) : base(controllerPackage)
		{
		}
	}
}
