using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdentityMVCDemo
{
    public class DemoUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<DemoUser>
    {
        public DemoUserClaimsPrincipalFactory(UserManager<DemoUser> userManager, 
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {

        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(DemoUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("locale", user.Locale));
            return identity;
        }
    }
}
