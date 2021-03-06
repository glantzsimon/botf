﻿using System.Data.Entity;
using K9.DataAccessLayer.Models;

namespace K9.DataAccessLayer.Database
{
    public class LocalDb : Base.DataAccessLayer.Database.Db
	{
        public DbSet<Donation> Donations { get; set; }
        public DbSet<ProjectDetail> ProjectDetails { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<MembershipOption> MembershipOptions { get; set; }
        public DbSet<UserMembership> UserMemberships { get; set; }
    }
}
