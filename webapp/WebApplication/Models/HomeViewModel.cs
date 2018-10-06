using System.Threading;

namespace K9.WebApplication.Models
{
    public class HomeViewModel
    {
        public string GabonLastDanceUrl
        {
            get
            {
                if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower() == "fr")
                {
                    return "https://www.youtube.com/embed/tJ8HIybl-fA";
                }

                return "https://www.youtube.com/embed/eXyAkjul5IA";
            }
        }
    }
}