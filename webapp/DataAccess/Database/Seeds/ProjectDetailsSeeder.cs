using System;
using K9.DataAccessLayer.Models;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using K9.SharedLibrary.Authentication;

namespace K9.DataAccessLayer.Database.Seeds
{
    public static class ProjectDetailsSeeder
    {
        public static void Seed(DbContext context)
        {
            if (!context.Set<ProjectDetail>().Any())
            {
                context.Set<ProjectDetail>().AddOrUpdate(new ProjectDetail
                {
                    NumberOfIbogasProjectedToBePlantedPerYear = 100,
                    NumberOfIbogasPlantedToDate = 500,
                    Name = Guid.NewGuid().ToString(),
                    CreatedBy = SystemUser.System,
                    CreatedOn = DateTime.Now
                });
            }
        }
    }
}
