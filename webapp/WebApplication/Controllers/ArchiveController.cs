using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.ViewModels;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using K9.DataAccessLayer.Models;
using K9.WebApplication.Extensions;
using K9.WebApplication.Models;
using K9.WebApplication.Services;

namespace K9.WebApplication.Controllers
{
    public class ArchiveController : BaseController
    {
        private readonly IRepository<ArchiveItemType> _archiveItemTypeRepo;
        private readonly IRepository<ArchiveItemCategory> _archiveItemCategoryRepo;
        private readonly IRepository<ArchiveItem> _archiveItemRepo;
        private readonly ILinkPreviewer _linkPreviewer;
        private readonly IMembershipService _membershipService;

        public ArchiveController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IRepository<ArchiveItemCategory> archiveItemCategoryRepo, IRepository<ArchiveItem> archiveItemRepo, IRepository<ArchiveItemType> archiveItemTypeRepo, IFileSourceHelper fileSourceHelper, ILinkPreviewer linkPreviewer, IMembershipService membershipService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
        {
            _archiveItemCategoryRepo = archiveItemCategoryRepo;
            _archiveItemRepo = archiveItemRepo;
            _linkPreviewer = linkPreviewer;
            _membershipService = membershipService;
            _archiveItemTypeRepo = archiveItemTypeRepo;
        }

        [OutputCache(Duration = 30, VaryByParam = "categoryId")]
        public ActionResult Index(int? categoryId)
        {
            if (categoryId.HasValue && !GetArchiveItemCategories().Select(c => c.Id).Contains(categoryId.Value))
            {
                return RedirectToAction("Index");
            }

            return View(GetArchiveViewModel(categoryId));
        }

        [Route("archive/{title}")]
        public ActionResult Details(string title)
        {
            var archiveItem = _archiveItemRepo.Find(e => e.SeoFriendlyId == title).FirstOrDefault();
            if (archiveItem == null)
            {
                return HttpNotFound();
            }

            var archiveModel = GetArchiveViewModel(archiveItem.CategoryId);
            var model = new ArchiveItemViewModel { ArchiveViewModel = archiveModel, SelectedItemId = archiveItem.Id };
            return View(model);
        }

        public JsonResult GetLinkPreview(string url)
        {
            try
            {
                return Json(_linkPreviewer.GetPreview(url, 600, 500), JsonRequestBehavior.AllowGet);
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

        private List<ArchiveItemCategory> GetArchiveItemCategories()
        {
            var activeUserMembership = _membershipService.GetActiveUserMembership();
            var itemCategories = _archiveItemCategoryRepo.List();
            if (activeUserMembership == null)
            {
                itemCategories = itemCategories.Where(_ => !_.IsSubscriptionOnly).ToList();
            }

            return itemCategories;
        }

        private ArchiveViewModel GetArchiveViewModel(int? categoryId)
        {
            var archiveItemTypes = _archiveItemTypeRepo.List();
            var archiveItemsToDisplay = _archiveItemRepo.Find(item => item.CategoryId == categoryId).ToList().Where(item => !item.IsShowLocalOnly || item.IsShowLocalOnly && item.LanguageCode == Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName)
                .Select(a =>
                {
                    a.ArchiveItemCategory = GetArchiveItemCategories().FirstOrDefault(c => c.Id == a.CategoryId);
                    a.ArchiveItemType = archiveItemTypes.FirstOrDefault(c => c.Id == a.TypeId);
                    return a;
                }).OrderByDescending(a => a.PublishedOn).ToList();

            var archiveModel = new ArchiveViewModel
            {
                CategoryId = categoryId ?? 0,
                ArchiveItemCategories = GetArchiveItemCategories()
                    .OrderBy(_ => _.Name)
                    .Select(a =>
                    {
                        var archiveItems = _archiveItemRepo.Find(item => item.CategoryId == a.Id).ToList().Where(item => !item.IsShowLocalOnly || item.IsShowLocalOnly && item.LanguageCode == Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName).ToList();
                        return new ArchiveByItemCategoryViewModel
                        {
                            ArchiveItemCategory = a,
                            Items = archiveItems
                        };
                    }).ToList(),
                //ArchiveItemTypes =  _archiveItemTypeRepo.List()
                //    .OrderBy(_ => _.Name)
                //    .Select(e =>
                //    {
                //        var archiveItems = _archiveItemRepo.Find(item => item.TypeId == e.Id).ToList().Where(item => !item.IsShowLocalOnly || item.IsShowLocalOnly && item.LanguageCode == Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName).ToList();
                //        return new ArchiveByItemTypeViewModel
                //        {
                //            ArchiveItemType = e,
                //            Items = archiveItems
                //        };
                //    }).ToList(),
                SelectedArchive = categoryId > 0 ? new ArchiveByItemCategoryViewModel
                {
                    ArchiveItemCategory = GetArchiveItemCategories().FirstOrDefault(_ => _.Id == categoryId),
                    Items = archiveItemsToDisplay
                } : null
            };

            archiveModel.SelectedArchive?.Items.ForEach(item => LoadUploadedFiles(item));

            return archiveModel;
        }
    }
}
