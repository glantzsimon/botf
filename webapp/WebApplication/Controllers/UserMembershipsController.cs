using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.Base.WebApplication.ViewModels;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using System.Web.Mvc;
using K9.DataAccessLayer.Models;

namespace K9.WebApplication.Controllers
{
    [Authorize]
	[RequirePermissions(Role = RoleNames.Administrators)]
	public class UserMembershipsController : BaseController<UserMembership>
	{
		private readonly IRepository<User> _userRepository;
		
		public UserMembershipsController(IControllerPackage<UserMembership> controllerPackage, IRepository<User> userRepository)
			: base(controllerPackage)
		{
			_userRepository = userRepository;
		}

		[Authorize]
		[RequirePermissions(Permission = Permissions.Edit)]
		public ActionResult EditUserMembershipsForUser(int id = 0)
		{
			return EditMultiple<User, MembershipOption>(_userRepository.Find(id));
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[RequirePermissions(Permission = Permissions.Edit)]
		public ActionResult EditUserMembershipsForUser(MultiSelectViewModel model)
		{
			return EditMultiple<User, MembershipOption>(model);
		}

	}
}
