using IdentityServer4.EntityFramework.Options;
//using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SchoolMeals.Shared.Models;

namespace SchoolMeals.Server.Data
{
    //public class AppDbContext : ApiAuthorizationDbContext<Users>
    public class AppDbContext : ApiAuthorizationDbContext<Users>
       
    {
        public AppDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {}

        public DbSet<Bookmarks> Bookmarks { get; set; }
        public DbSet<Meal> Meal { get; set; }
        public DbSet<Municipality> Municipality { get; set; }
        public DbSet<Recipe> Recipe { get; set; }
        public DbSet<School> School { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<IdentityUser>(b => { b.HasNoKey(); b.ToView("Users"); b.ToTable("Users"); });
            //modelBuilder.Entity<IdentityUserClaim<string>>(b => { b.HasNoKey(); b.ToView("UsersClaims"); b.ToTable("UserClaims"); });
            //modelBuilder.Entity<IdentityUserLogin<string>>(b => { b.HasNoKey(); b.ToView("UsersLogins"); b.ToTable("UserLogins"); });
            //modelBuilder.Entity<IdentityRole>(b => { b.HasNoKey(); b.ToView("Roles"); b.ToTable("Roles"); });
            //modelBuilder.Entity<IdentityUserRole<string>>(b => { b.HasNoKey(); b.ToView("UserRoles"); b.ToTable("UserRoles"); });

            //seed roles
            //modelBuilder.Entity<IdentityRole>().HasData(
            //        new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
            //        new IdentityRole { Name = "Support", NormalizedName = "SUPPORT" },
            //        new IdentityRole { Name = "Customer", NormalizedName = "CUSTOMER" },
            //        new IdentityRole { Name = "User", NormalizedName = "USER" }
            //  );

            base.OnModelCreating(modelBuilder);
        }
    }
}
