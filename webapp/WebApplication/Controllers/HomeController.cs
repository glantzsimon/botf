using K9.Base.WebApplication.Controllers;
using K9.SharedLibrary.Models;
using NLog;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class HomeController : BaseController
	{

		public HomeController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication)
			: base(logger, dataSetsHelper, roles, authentication)
		{
		}

		public ActionResult Index()
		{
		    return View();
		}

		public ActionResult SetLanguage(string languageCode)
		{
			Session["languageCode"] = languageCode;
			return RedirectToAction("Index");
		}
		
		public override string GetObjectName()
		{
			return string.Empty;
		}
	}
}
