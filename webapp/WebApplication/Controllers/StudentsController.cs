using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;

namespace K9.WebApplication.Controllers
{
    public class StudentsController : BaseController<Student>
	{
		public StudentsController(IControllerPackage<Student> controllerPackage) : base(controllerPackage)
		{
		}
	}
}
