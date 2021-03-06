﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;

namespace AppPortal.Infrastructure.Identity
{
    public class AppIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
		public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
              : base(options)
        {
		}

		public class ApplicationContextDbFactory : IDesignTimeDbContextFactory<AppIdentityDbContext>
		{
			public ApplicationContextDbFactory(IConfiguration configuration)
			{
				Configuration = configuration;
			}

			public IConfiguration Configuration { get; }
			AppIdentityDbContext IDesignTimeDbContextFactory<AppIdentityDbContext>.CreateDbContext(string[] args)
			{
				var optionsBuilder = new DbContextOptionsBuilder<AppIdentityDbContext>();
				optionsBuilder.UseSqlServer<AppIdentityDbContext>(Configuration.GetConnectionString("IdentityConnection"));

				return new AppIdentityDbContext(optionsBuilder.Options);
			}
		}

		protected override void OnModelCreating(ModelBuilder builder)
        {
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>(ConfigureUser);
            builder.Entity<ApplicationRole>(ConfigureRole);
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>(b => b.ToTable("RoleClaims", "Security"));
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>(ConfigureUserRole);
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>(b => b.ToTable("UserLogins", "Security"));
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>(b => b.ToTable("UserTokens", "Security"));
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>(b => b.ToTable("UserClaims", "Security"));
        }

        private void ConfigureRole(EntityTypeBuilder<ApplicationRole> b)
        {
            b.ToTable("Roles", "Security");
            b.Property(c => c.RoleDescription).HasMaxLength(255);           
        }

        private void ConfigureUser(EntityTypeBuilder<ApplicationUser> b)
        {
            b.ToTable("Users", "AppPortal");
            b.HasMany(x => x.Roles).WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
            b.Property(c => c.EmailAuth).HasColumnType("ntext");
        }

        private void ConfigureUserRole(EntityTypeBuilder<Microsoft.AspNetCore.Identity.IdentityUserRole<string>> b)
        {
            b.ToTable("UserRoles", "Security");
        }
    }
}
