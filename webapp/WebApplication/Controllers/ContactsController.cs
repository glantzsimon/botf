using System.Linq;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using System.Web.Mvc;
using K9.SharedLibrary.Models;

namespace K9.WebApplication.Controllers
{
    [Authorize]
	[RequirePermissions(Role = RoleNames.Administrators)]
	public class ContactsController : BaseController<Contact>
	{
	    private readonly IRepository<Donation> _donationRepository;

	    public ContactsController (IControllerPackage<Contact> controllerPackage, IRepository<Donation> donationRepository) : base(controllerPackage)
	    {
	        _donationRepository = donationRepository;
	    }

	    public ActionResult ImportContactsFromDonations()
	    {
	        var existing = Repository.List();

	        var contactsToImport = _donationRepository.List().Where(c => !string.IsNullOrEmpty(c.CustomerEmail) && existing.All(e => e.EmailAddress != c.CustomerEmail))
	            .Select(e => new Contact
	            {
	                FullName = e.CustomerName,
	                EmailAddress = e.CustomerEmail
	            }).ToList();

	        Repository.CreateBatch(contactsToImport);

	        return RedirectToAction("Index");
	    }
	}
}
