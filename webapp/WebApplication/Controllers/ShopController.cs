using K9.Base.WebApplication.Controllers;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using NLog;
using System.Threading;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class ShopController : BaseController
	{

		public ShopController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper)
			: base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
		{
		}

		public ActionResult Index()
		{
		    ViewData[Constants.ViewDataConstants.Locale] =
		        Thread.CurrentThread.CurrentUICulture.GetMetaLocaleName("_");
		    return View();
		}
		
		public override string GetObjectName()
		{
			return string.Empty;
		}
	}
}
