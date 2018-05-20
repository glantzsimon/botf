﻿using K9.Base.DataAccessLayer.Database;
using K9.DataAccessLayer.Database;
using K9.DataAccessLayer.Database.Seeds;
using System.Data.Entity.Migrations;
using WebMatrix.WebData;

namespace K9.WebApplication
{
    public class DataConfig
    {
        public static void InitialiseDatabase()
        {
            #if DEBUG
            var migrator = new DbMigrator(new DatabaseInitialiserLocal());
            migrator.Update();
            #endif
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