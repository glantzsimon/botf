using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.UnitsOfWork;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
	public class ArchiveItemCategoriesController : BaseController<ArchiveItemCategory>
	{
		public ArchiveItemCategoriesController(IControllerPackage<ArchiveItemCategory> controllerPackage) : base(controllerPackage)
		{
		}
	}
}
