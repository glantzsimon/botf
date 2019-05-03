using K9.Base.WebApplication.Controllers;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Extensions;
using NLog;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class SharedController : BaseController
	{

		public SharedController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper)
			: base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
		{
		}

	    public override string GetObjectName()
	    {
	        throw new System.NotImplementedException();
	    }

	    public JsonResult GetSeoFriendlyName(string value)
	    {
	        return Json(value.ToSeoFriendlyString(), JsonRequestBehavior.AllowGet);
	    }
	}
}
