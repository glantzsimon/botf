using K9.Base.WebApplication.Controllers;
using K9.SharedLibrary.Models;
using NLog;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class ArchiveController : BaseController
	{

		public ArchiveController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication)
			: base(logger, dataSetsHelper, roles, authentication)
		{
		}

		public ActionResult Index()
		{
			return View();
		}
		
		public override string GetObjectName()
		{
			return string.Empty;
		}
	}
}
