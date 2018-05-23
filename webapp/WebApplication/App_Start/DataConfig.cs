using System.Configuration;
using K9.Base.DataAccessLayer.Database;
using K9.DataAccessLayer.Database;
using K9.DataAccessLayer.Database.Seeds;
using System.Data.Entity.Migrations;
using K9.Base.DataAccessLayer.Config;
using K9.SharedLibrary.Helpers;
using WebMatrix.WebData;

namespace K9.WebApplication
{
    public class DataConfig
    {
        public static void InitialiseDatabase()
        {
            var dbConfig = ConfigHelper.GetConfiguration<DatabaseConfiguration>(ConfigurationManager.AppSettings).Value;
            if (dbConfig.AutomaticMigrationsEnabled)
            {
                var migrator = new DbMigrator(new DatabaseInitialiserLocal());
                migrator.Update();
            }
        }

        public static void InitialiseWebsecurity()
        {
            if (!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection("DefaultConnection", "User", "Id", "UserName", true);
            }
        }

        public static void InitialiseUsersAndRoles()
        {
            UsersAndRolesInitialiser.Seed();
            var dbContext = new LocalDb();
            PermissionsSeeder.Seed(dbContext);
        }
    }
}