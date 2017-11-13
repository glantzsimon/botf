using System.Web.Mvc;
using K9.Base.WebApplication.Controllers;
using K9.SharedLibrary.Models;
using NLog;

namespace K9.WebApplication.Controllers
{
	public class AboutUsController : BaseController
	{

		public AboutUsController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication)
			: base(logger, dataSetsHelper, roles, authentication)
		{
		}

		public ActionResult Index()
		{
			return View();
		}
        
		public ActionResult Team()
		{
			return View();
		}

		public ActionResult HelpUs()
		{
			return View();
		}

		public override string GetObjectName()
		{
			return string.Empty;
		}
	}
}
