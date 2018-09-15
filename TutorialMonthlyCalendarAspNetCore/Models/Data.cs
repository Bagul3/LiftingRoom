using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace TutorialMonthlyCalendarAspNetCore.Models
{
    public class CalendarDbContext : DbContext
    {
        public DbSet<CalendarEvent> Events { get; set; }

        public CalendarDbContext(DbContextOptions<CalendarDbContext> options):base(options) { }

        public DbSet<Memberships> Memberships { get; set; }

        public DbSet<TutorialMonthlyCalendarAspNetCore.Models.Package> Package { get; set; }

        public DbSet<MembershipPackage> MembershipPackages { get; set; }
    }

    public class CalendarEvent
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        public string MembershipNumber { get; set; }
        public string MembershipName { get; set; }
        public string PackageName { get; set; }
        public string PackageNumber { get; set; }
    }

    public class Memberships
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Package { get; set; }

        public string EmergencyName { get; set; }

        public string EmergencyContact { get; set; }

        public string Notes { get; set; }
    }

}
