using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.EventArgs;
using K9.Base.WebApplication.UnitsOfWork;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using System;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    public class ArchiveItemsController : BaseController<ArchiveItem>
    {
        private readonly IRepository<ArchiveCategory> _archiveCategoriesRepository;

        public ArchiveItemsController(IControllerPackage<ArchiveItem> controllerPackage, IRepository<ArchiveCategory> archiveCategoriesRepository)
            : base(controllerPackage)
        {
            _archiveCategoriesRepository = archiveCategoriesRepository;
            RecordBeforeCreate += ArchiveItemsController_RecordBeforeCreate;
            RecordBeforeCreated += ArchiveItemsController_RecordBeforeUpdated;
            RecordBeforeUpdate += ArchiveItemsController_RecordBeforeUpdate;
            RecordBeforeUpdated += ArchiveItemsController_RecordBeforeUpdated;
        }
        
        private void ArchiveItemsController_RecordBeforeUpdated(object sender, CrudEventArgs e)
        {
            var archiveItem = e.Item as ArchiveItem;
            if (!string.IsNullOrEmpty(archiveItem.Url))
            {
                archiveItem.Url = HelperMethods.GetEmbeddableUrl(archiveItem.Url);
            }
        }

        void ArchiveItemsController_RecordBeforeCreate(object sender, CrudEventArgs e)
        {
            var archiveItem = e.Item as ArchiveItem;
            archiveItem.PublishedBy = WebSecurity.IsAuthenticated ? WebSecurity.CurrentUserName : string.Empty;
            archiveItem.PublishedOn = DateTime.Now;
        }

        private void ArchiveItemsController_RecordBeforeUpdate(object sender, CrudEventArgs e)
        {
            var archiveItem = e.Item as ArchiveItem;
            archiveItem.ArchiveCategory = _archiveCategoriesRepository.Find(archiveItem.CategoryId);
        }

    }
}