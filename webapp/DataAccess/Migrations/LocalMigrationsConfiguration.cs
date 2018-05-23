using K9.DataAccessLayer.Database;
using System.Data.Entity.Migrations;

namespace K9.DataAccessLayer.Migrations
{
    public sealed class LocalMigrationsConfiguration : DbMigrationsConfiguration<LocalDb>
    {
        public LocalMigrationsConfiguration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
        }
    }
}
