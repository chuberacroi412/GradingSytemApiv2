using GradingSytemApi.Common.Helpers;
using GradingSytemApi.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi.Entities
{
    public class ApiDbContext : IdentityDbContext<Account, Role, string, IdentityUserClaim<string>, 
                                AccountRoleMap, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {

        private readonly UserResolverService _userResolverService;

        public ApiDbContext(DbContextOptions<ApiDbContext> options, UserResolverService userResolverService) : base(options)
        {
            _userResolverService = userResolverService;
        }

        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<Module> Modules { get; set; }
        public DbSet<RoleModuleMap> RoleModuleMaps { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Score> Scrores { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries();
            List<EntityEntry> modifiedEntries = new List<EntityEntry>();

            // Find modified entries
            foreach(var entry in entries)
            {
                if(entry.Entity is IAuditableEntity && entry.State == EntityState.Modified || entry.State == EntityState.Added)
                {
                    modifiedEntries.Add(entry);
                }
            }

            // Update information for modified entries
            foreach(var entry in modifiedEntries)
            {
                DateTime now = DateTime.Now;

                if(entry is IAuditableEntity entity)
                {
                    string identityName = _userResolverService.GetUser();

                    if(entry.State == EntityState.Added)
                    {
                        entity.CreatedBy = string.IsNullOrEmpty(entity.CreatedBy) ? identityName : entity.CreatedBy;
                        entity.CreatedDate = now;
                    }
                    else
                    {
                        base.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                        base.Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                    }

                    entity.UpdatedBy = identityName;
                    entity.UpdatedDate = now;
                }
            }

            return base.SaveChanges();
        }
    }
}
