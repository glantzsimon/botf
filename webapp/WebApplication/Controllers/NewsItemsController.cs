using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.EventArgs;
using K9.Base.WebApplication.UnitsOfWork;
using K9.SharedLibrary.Attributes;
using System;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace K9.WebApplication.Controllers
{
    [Authorize]
	[LimitByUserId]
	public class NewsItemsController : BaseController<NewsItem>
	{

		public NewsItemsController(IControllerPackage<NewsItem> controllerPackage)
			: base(controllerPackage)
		{
			RecordBeforeCreate += NewsItemsController_RecordBeforeCreate;
		}
		
		void NewsItemsController_RecordBeforeCreate(object sender, CrudEventArgs e)
		{
			var newsItem = e.Item as NewsItem;
			newsItem.PublishedBy = WebSecurity.IsAuthenticated ? WebSecurity.CurrentUserName : string.Empty;
			newsItem.PublishedOn = DateTime.Now;
		}
	
	}
}
