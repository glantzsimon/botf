using System.Globalization;
using System.Linq;
using System.Threading;
using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Controllers;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Constants;
using NLog;
using System.Web.Mvc;
using K9.WebApplication.Extensions;

namespace K9.WebApplication.Controllers
{
    public class NewsController : BaseController
    {
        private readonly IRepository<NewsItem> _newsRepository;

        public NewsController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IRepository<NewsItem> newsRepository, IAuthentication authentication, IFileSourceHelper fileSourceHelper)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
        {
            _newsRepository = newsRepository;
        }

        public ActionResult Index(int id = 0)
        {
            ViewData[ViewDataConstants.SelectedId] = id;
            var newsItems = _newsRepository.List()
                                            .Where(item => !item.IsShowLocalOnly || item.IsShowLocalOnly && item.LanguageCode == Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName)
                                            .OrderByDescending(n => n.PublishedOn).ToList();
            newsItems.ForEach(item => LoadUploadedFiles(item));
            return View(newsItems);
        }

        public ActionResult NewsSummary()
        {
            return PartialView("_NewsSummary", _newsRepository.GetQuery("SELECT TOP 10 * FROM [NewsItem] ORDER BY [PublishedOn] DESC").Where(item => !item.IsShowLocalOnly || item.IsShowLocalOnly && item.LanguageCode == Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName).ToList());
        }

        [Route("news/{subject}")]
        public ActionResult Details(string subject)
        {
            var model = _newsRepository.Find(e => e.SeoFriendlyId == subject).FirstOrDefault();
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        public override string GetObjectName()
        {
            return typeof(NewsItem).Name;
        }
    }
}
