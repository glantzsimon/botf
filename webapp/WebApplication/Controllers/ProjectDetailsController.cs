using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.EventArgs;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using K9.WebApplication.Services;

namespace K9.WebApplication.Controllers
{
    public class ProjectDetailsController : BaseController<ProjectDetail>
    {
        private readonly IDonationService _donationService;

        public ProjectDetailsController(IControllerPackage<ProjectDetail> controllerPackage, IDonationService donationService)
            : base(controllerPackage)
        {
            _donationService = donationService;
            RecordBeforeCreate += ProjectDetailsController_RecordBeforeCreate;
        }

        void ProjectDetailsController_RecordBeforeCreate(object sender, CrudEventArgs e)
        {
            if (Repository.GetCount() > 0)
            {
                throw new DuplicateNameException("Only one project statistics can be created");
            }
        }

        [OutputCache(Duration = 10, VaryByParam = "none")]
        public JsonResult GetProjectStatistics()
        {
            return Json(new
            {
                ProjectDetails?.NumberOfIbogasPlantedToDate,
                NumberOfIbogasProjectedToBePlantedPerYear = ProjectDetails?.NumberOfIbogasProjectedToBePlantedThisYear + _donationService.GetProjectedNumberOfIbogasSponsoredPerYear(),
                NumberOfIbogasSponsoredToDate = _donationService.GetNumberOfIbogasSponsoredToDate() + ProjectDetails?.NumberOfIbogasSponsoredToDate,
                ProjectDetails?.NumberOfIbogasProjectedToBePlantedThisYear,
                ProjectDetails?.NumberOfIbogasProjectedToBePlantedTwoYears
            }, JsonRequestBehavior.AllowGet);
        }

        private ProjectDetail ProjectDetails => Repository.List().FirstOrDefault();
    }
}