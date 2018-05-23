using System.Configuration;
using K9.Base.DataAccessLayer.Config;
using K9.SharedLibrary.Helpers;

namespace K9.DataAccessLayer.Migrations
{
    using System.Data.Entity.Migrations;

    public sealed class LocalConfiguration : DbMigrationsConfiguration<Database.LocalDb>
    {
        public LocalConfiguration()
        {
            var dbConfig = ConfigHelper.GetConfiguration<DatabaseConfiguration>(ConfigurationManager.AppSettings).Value;
            AutomaticMigrationsEnabled = dbConfig.AutomaticMigrationsEnabled;
        }

        protected override void Seed(Database.LocalDb context)
        {
        }
    }
}
