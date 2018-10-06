using System.Threading;
using K9.Base.WebApplication.Controllers;
using K9.SharedLibrary.Models;
using NLog;
using System.Web.Mvc;
using K9.Base.Globalisation;
using K9.Base.WebApplication.Constants;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Models;

namespace K9.WebApplication.Controllers
{
    public class HomeController : BaseController
	{

		public HomeController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper)
			: base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
		{
		}

		public ActionResult Index()
		{
		    FixFrenchBug();
		    return View(new HomeViewModel());
		}

		public ActionResult SetLanguage(string languageCode, string cultureCode)
		{
			Session[SessionConstants.LanguageCode] = languageCode;
		    Session[SessionConstants.CultureCode] = cultureCode;
		    return Redirect(Request.UrlReferrer?.ToString());
		}
		
		public override string GetObjectName()
		{
			return string.Empty;
		}

	    private void FixFrenchBug()
	    {
	        if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower() == "fr")
	        {
	            Session[SessionConstants.LanguageCode] = Strings.LanguageCodes.France;
	            Session[SessionConstants.CultureCode] = Strings.CultureCodes.French;
	        }
	    }
	}
}
