using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.ViewModels;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class ArchiveController : BaseController
    {
        private readonly IRepository<ArchiveItemType> _archiveItemTypeRepo;
        private readonly IRepository<ArchiveItemCategory> _archiveItemCategoryRepo;
        private readonly IRepository<ArchiveItem> _archiveItemRepo;
        private readonly ILinkPreviewer _linkPreviewer;

        public ArchiveController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IRepository<ArchiveItemCategory> archiveItemCategoryRepo, IRepository<ArchiveItem> archiveItemRepo, IRepository<ArchiveItemType> archiveItemTypeRepo, IFileSourceHelper fileSourceHelper, ILinkPreviewer linkPreviewer)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
        {
            _archiveItemCategoryRepo = archiveItemCategoryRepo;
            _archiveItemRepo = archiveItemRepo;
            _linkPreviewer = linkPreviewer;
            _archiveItemTypeRepo = archiveItemTypeRepo;
        }

        [OutputCache(Duration = 30, VaryByParam = "categoryId")]
        public ActionResult Index(int? categoryId)
        {
            var archiveItemCategories = _archiveItemCategoryRepo.List();
            var archiveItemTypes = _archiveItemTypeRepo.List();
            var archiveItemsToDisplay = _archiveItemRepo.Find(item => item.CategoryId == categoryId).ToList()
                .Select(a =>
                {
                    a.ArchiveItemCategory = archiveItemCategories.FirstOrDefault(c => c.Id == a.CategoryId);
                    a.ArchiveItemType = archiveItemTypes.FirstOrDefault(c => c.Id == a.TypeId);
                    return a;
                }).OrderByDescending(a => a.PublishedOn).ToList();
            var archiveModel = new ArchiveViewModel
            {
                CategoryId = categoryId ?? 0,
                ArchiveItemCategories = _archiveItemCategoryRepo.List()
                    .OrderBy(_ => _.Name)
                    .Select(a =>
                    {
                        var archiveItems = _archiveItemRepo.Find(item => item.CategoryId == a.Id).ToList();
                        return new ArchiveByItemCategoryViewModel
                        {
                            ArchiveItemCategory = a,
                            Items = archiveItems
                        };
                    }).ToList(),
                ArchiveItemTypes =  _archiveItemTypeRepo.List()
                    .OrderBy(_ => _.Name)
                    .Select(e =>
                    {
                        var archiveItems = _archiveItemRepo.Find(item => item.TypeId == e.Id).ToList();
                        return new ArchiveByItemTypeViewModel
                        {
                            ArchiveItemType = e,
                            Items = archiveItems
                        };
                    }).ToList(),
                SelectedArchive = categoryId > 0 ? new ArchiveByItemCategoryViewModel
                {
                    ArchiveItemCategory = archiveItemCategories.FirstOrDefault(_ => _.Id == categoryId),
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
