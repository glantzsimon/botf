using System.Collections.Generic;
using System.Linq;
using K9.Base.WebApplication.Controllers;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using NLog;
using System.Web.Mvc;
using K9.DataAccessLayer.Models;
using K9.WebApplication.Models;

namespace K9.WebApplication.Controllers
{
    public class MembershipController : BaseController
    {
        private readonly IRepository<MembershipOption> _membershipOptionRepository;
        private readonly IRepository<UserMembership> _userMembershipRepository;

        public MembershipController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IRepository<MembershipOption> membershipOptionRepository, IRepository<UserMembership> userMembershipRepository)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
        {
            _membershipOptionRepository = membershipOptionRepository;
            _userMembershipRepository = userMembershipRepository;
        }

        public ActionResult Index()
        {
            var membershipOptions = _membershipOptionRepository.List();
            var userMemberships = Authentication.IsAuthenticated
                ? _userMembershipRepository.Find(_ => _.UserId == Authentication.CurrentUserId).ToList()
                : new List<UserMembership>();
            var model = new List<MembershipModel>(membershipOptions.Select(membershipOption =>
                {
                    return new MembershipModel(membershipOption,
                        userMemberships.FirstOrDefault(_ => _.UserId == Authentication.CurrentUserId));
                }));

            return View(model);
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }

    }
}
