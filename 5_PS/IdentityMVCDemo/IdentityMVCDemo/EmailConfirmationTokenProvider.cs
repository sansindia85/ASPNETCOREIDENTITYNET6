using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdentityMVCDemo
{
    public class EmailConfirmationTokenProvider<TUser> :
        DataProtectorTokenProvider<TUser> where TUser : class
    {
        public EmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
            IOptions<EMailConfirmationTokenProviderOptions> options,
            ILogger<DataProtectorTokenProvider<TUser>> logger) : 
            base(dataProtectionProvider, options, logger )
        {

        }
    }

    public class EMailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
    {

    }
}
