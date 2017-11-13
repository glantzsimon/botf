using K9.Base.DataAccessLayer.Config;
using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.EventArgs;
using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using System.Linq;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace K9.WebApplication.Controllers
{
    [Authorize]
	[RequirePermissions(Role = RoleNames.Administrators)]
	public class UsersController : BaseController<User>
	{
		private readonly IOptions<DatabaseConfiguration> _dataConfig;
		private readonly IRepository<Message> _messageRepository;

		public UsersController(IControllerPackage<User> controllerPackage, IOptions<DatabaseConfiguration> dataConfig, IRepository<Message> messageRepository)
			: base(controllerPackage)
		{
			_dataConfig = dataConfig;
			_messageRepository = messageRepository;
			RecordCreated += UsersController_RecordCreated;
			RecordBeforeDelete += UsersController_RecordBeforeDelete;
		}

		void UsersController_RecordBeforeDelete(object sender, CrudEventArgs e)
		{
			var user = e.Item as User;
			DeleteLinkedRecords(user);
		}

		void UsersController_RecordCreated(object sender, CrudEventArgs e)
		{
			var user = e.Item as User;
			WebSecurity.CreateAccount(user.Username, _dataConfig.Value.DefaultUserPassword, !user.AccountActivated);
		}

		private void DeleteLinkedRecords(User user)
		{
			var messages = _messageRepository.Find(m => m.UserId == user.Id).ToList();
			_messageRepository.DeleteBatch(messages);
		}

	}
}
