using System.ComponentModel.DataAnnotations;

namespace IdentityMVCDemo.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
