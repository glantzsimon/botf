namespace K9.WebApplication.Services
{
    public interface IMailChimpService
    {
        void AddContact(string firstName, string lastName, string emailAddress);
        void AddAllContacts();
    }
}