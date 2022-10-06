using Microsoft.AspNetCore.Identity;

namespace IdentityMVCDemo
{
    public class DemoUser : IdentityUser
    {
        public string Locale { get; set; } = "en-IN";
        public string OrgId { get; set; }
    }

    public class Organization
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
