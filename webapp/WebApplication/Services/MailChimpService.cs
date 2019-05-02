using K9.SharedLibrary.Models;
using MailChimp.Net;
using MailChimp.Net.Models;
using System.Collections.Generic;
using System.Linq;
using MailChimpConfiguration = K9.WebApplication.Config.MailChimpConfiguration;

namespace K9.WebApplication.Services
{
    public class MailChimpService : IMailChimpService
    {
        private readonly IContactService _contactService;
        private readonly MailChimpConfiguration _mailChimpConfig;

        public MailChimpService(IOptions<MailChimpConfiguration> mailChimpConfig, IContactService contactService)
        {
            _contactService = contactService;
            _mailChimpConfig = mailChimpConfig.Value;
        }

        public void AddContact(string firstName, string lastName, string emailAddress)
        {
            var mailChimpManager = new MailChimpManager(_mailChimpConfig.MailChimpApiKey);

            mailChimpManager.Members.AddOrUpdateAsync(_mailChimpConfig.MailChimpListId, new Member
            {
                EmailAddress = emailAddress,
                Status = Status.Subscribed,
                StatusIfNew = Status.Subscribed,
                MergeFields = new Dictionary<string, object>
                {
                    {"FNAME", firstName},
                    {"LNAME", lastName},
                }
            });
        }

        public void AddAllContacts()
        {
            foreach (var contact in _contactService.ListContacts())
            {
                var nameParts = contact.FullName.Split(' ');
                AddContact(nameParts.FirstOrDefault(), nameParts.LastOrDefault(), contact.EmailAddress);
            }
        }
    }
}