using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using WebApi.Entities;
using WebApi.Models;
using WebApi.Utils;

namespace WebApi.DataAccess.Base
{
    //public class SqlContext : DbContext
    public class SqlContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public SqlContext(DbContextOptions<SqlContext> options)
        : base(options){ }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Core Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Core Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            //builder.Entity<ApplicationUser>(entity => entity.ToTable("user", "tl_wiki"));

            //builder.Entity<MentItems>().Property(e => e.IsValid)
            //    .HasColumnType("bit(1)")
            //    .HasDefaultValue(false);


            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(bool))
                    {
                        property.SetValueConverter(new BoolToIntConverter());
                    }
                }
            }
        }

        

        public DbSet<User> User { get; set; }
        public DbSet<Topic> Wiki_topic { get; set; }
        public DbSet<Wiki> Wiki_item { get; set; }
        public DbSet<Category> Topic_category { get; set; }
        public DbSet<AccountRole> aspnetuserroles { get; set; }
        public DbSet<Userclaims> aspnetuserclaims { get; set; }


    }
}
