using DuendeWithIdentity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DuendeWithIdentity.Data
{
    public class DemoUserDbContext : IdentityDbContext<DemoUser>
    {
        public DemoUserDbContext(DbContextOptions<DemoUserDbContext> options) : base(options)
        {

        }
    }
}
