using K9.Base.DataAccessLayer.Config;
using K9.Base.DataAccessLayer.Database;
using K9.SharedLibrary.Helpers;
using System.Configuration;

namespace K9.DataAccessLayer.Database
{
    public class DatabaseInitialiserLocal : DatabaseInitialiser<LocalDb>
	{
	    public DatabaseInitialiserLocal()
	    {
	        var dbConfig = ConfigHelper.GetConfiguration<DatabaseConfiguration>(ConfigurationManager.AppSettings).Value;
	        AutomaticMigrationsEnabled = dbConfig.AutomaticMigrationsEnabled;
	        AutomaticMigrationDataLossAllowed = dbConfig.AutomaticMigrationDataLossAllowed;
	    }

        protected override void Seed(LocalDb db)
		{
            base.Seed(db);
		}
	}
}
