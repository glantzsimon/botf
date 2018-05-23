using K9.Base.DataAccessLayer.Config;
using K9.Base.DataAccessLayer.Database;
using K9.DataAccessLayer.Database;
using K9.DataAccessLayer.Database.Seeds;
using K9.DataAccessLayer.Migrations;
using K9.SharedLibrary.Helpers;
using System.Configuration;
using System.Data.Entity.Migrations;
using WebMatrix.WebData;

namespace K9.WebApplication
{
    public class DataConfig
    {
        public static void InitialiseDatabase()
        {
            var dbConfig = ConfigHelper.GetConfiguration<DatabaseConfiguration>(ConfigurationManager.AppSettings).Value;
            if (dbConfig.IsInitialCreate)
            {
                var migrator = new DbMigrator(new DatabaseInitialiser<LocalDb>());
                migrator.Update();
            }
            else
            {
                #if DEBUG
                var migrator = new DbMigrator(new LocalMigrationsConfiguration());
                migrator.Update();
                #endif
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
            PermissionsSeeder.Seed(new LocalDb());
        }
    }
}