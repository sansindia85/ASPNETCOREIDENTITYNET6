using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityMVCDemo
{
    public class DemoUserDbContext : IdentityDbContext<DemoUser>
    {
        public DemoUserDbContext(DbContextOptions<DemoUserDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<DemoUser>(user => user.HasIndex(x => x.Locale).IsUnique(false));

            builder.Entity<Organization>(org =>
            {
                org.ToTable("Organizations");
                org.HasKey(x => x.Id);

                org.HasMany<DemoUser>().WithOne().HasForeignKey(x => x.OrgId).IsRequired(false);
            });

        }
    }
}
