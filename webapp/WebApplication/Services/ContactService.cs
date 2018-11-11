using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using NLog;
using System;

namespace K9.WebApplication.Services
{
    public class ContactService : IContactService
    {
        private readonly IRepository<Contact> _contactsRepository;
        private readonly ILogger _logger;

        public ContactService(IRepository<Contact> contactsRepository, ILogger logger)
        {
            _contactsRepository = contactsRepository;
            _logger = logger;
        }

        public void CreateCustomer(string stripeCustomerId, string fullName, string emailAddress)
        {
            if (!string.IsNullOrEmpty(emailAddress))
            {
                try
                {
                    _contactsRepository.Create(new Contact
                    {
                        StripeCustomerId = stripeCustomerId,
                        FullName = string.IsNullOrEmpty(fullName) ? emailAddress : fullName,
                        EmailAddress = emailAddress
                    });
                }
                catch (Exception e)
                {
                    _logger.Error($"ContactService => CreateCustomer => {e.Message}");
                    throw;
                }
            }
        }
    }
}