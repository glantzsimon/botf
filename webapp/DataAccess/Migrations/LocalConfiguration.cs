namespace K9.DataAccessLayer.Migrations
{
    using System.Data.Entity.Migrations;

    public sealed class LocalConfiguration : DbMigrationsConfiguration<Database.LocalDb>
    {
        public LocalConfiguration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(Database.LocalDb context)
        {
        }
    }
}
