using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Services.Stripe;
using System;
using System.Linq;
using System.Threading;
using System.Web.Hosting;
using K9.Base.DataAccessLayer.Respositories;
using K9.DataAccessLayer.Database;

namespace K9.WebApplication
{
    public class JobScheduler : IRegisteredObject
    {
        private readonly IStripeService _stripeService;
        private readonly TaskConfiguration _config;
        private Timer _timer;

        public JobScheduler(IOptions<TaskConfiguration> config, IStripeService stripeService)
        {
            _stripeService = stripeService;
            _config = config.Value;
            StartTimer();
        }

        public void Stop(bool immediate)
        {
            _timer.Dispose();
            HostingEnvironment.UnregisterObject(this);
        }

        private void StartTimer()
        {
            var delayStartby = TimeSpan.FromSeconds(_config.DelayStartBySeconds);
            var repeatEvery = TimeSpan.FromSeconds(_config.RepeatEverySeconds);

            _timer = new Timer(RunTasks, null, delayStartby, repeatEvery);
        }

        private void RunTasks(object state)
        {
            
        }

        private void UpdateDonations()
        {
            using (var dbContext = new LocalDb())
            {
                var repository = new BaseRepository<Donation>(dbContext);
                var existingDonations = repository.List();
                var newDonations = _stripeService.GetDonations();
                
                foreach (var donation in newDonations)
                {
                    if (!existingDonations.Exists(d => d.StripeId == donation.StripeId))
                    {
                        repository.Create(donation);
                    }
                    else
                    {
                        var entity = repository.Find(e => e.StripeId == donation.StripeId).FirstOrDefault();
                        if (entity != null)
                        {
                            entity.Status = donation.Status;
                            repository.Update(entity);
                        }
                    }
                }
            }
        }
    }
}