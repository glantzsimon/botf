using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.ViewModels;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using NLog;
using System;
using System.Linq;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class ArchiveController : BaseController
    {
        private readonly IRepository<ArchiveCategory> _archiveCategoryRepo;
        private readonly IRepository<ArchiveItem> _archiveItemRepo;
        private readonly ILinkPreviewer _linkPreviewer;

        public ArchiveController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IRepository<ArchiveCategory> archiveCategoryRepo, IRepository<ArchiveItem> archiveItemRepo, IFileSourceHelper fileSourceHelper, ILinkPreviewer linkPreviewer)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
        {
            _archiveCategoryRepo = archiveCategoryRepo;
            _archiveItemRepo = archiveItemRepo;
            _linkPreviewer = linkPreviewer;
        }

        [OutputCache(Duration = 30, VaryByParam = "categoryId")]
        public ActionResult Index(int? categoryId)
        {
            var archiveCategories = _archiveCategoryRepo.List();
            var archiveItemsToDisplay = _archiveItemRepo.Find(item => item.CategoryId == categoryId).ToList()
                .Select(a =>
                {
                    a.ArchiveCategory = archiveCategories.FirstOrDefault(c => c.Id == a.CategoryId);
                    return a;
                }).OrderByDescending(a => a.PublishedOn).ToList();
            var archiveModel = new ArchiveViewModel
            {
                CategoryId = categoryId ?? 0,
                ArchiveCategories = _archiveCategoryRepo.List()
                    .OrderBy(_ => _.Name)
                    .Select(a =>
                    {
                        var archiveItems = _archiveItemRepo.Find(item => item.CategoryId == a.Id).ToList();
                        return new ArchiveCategoryViewModel
                        {
                            ArchiveCategory = a,
                            Items = archiveItems
                        };
                    }).ToList(),
                SelectedArchive = categoryId > 0 ? new ArchiveCategoryViewModel
                {
                    ArchiveCategory = archiveCategories.FirstOrDefault(_ => _.Id == categoryId),
                    Items = archiveItemsToDisplay
                } : null
            };
            archiveModel.SelectedArchive?.Items.ForEach(item => LoadUploadedFiles(item));
            return View(archiveModel);
        }

        public JsonResult GetLinkPreview(string url)
        {
            try
            {
                return Json(_linkPreviewer.GetPreview(url), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new LinkPreviewResult(url, string.Empty, string.Empty, string.Empty), JsonRequestBehavior.AllowGet);
            }
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}
