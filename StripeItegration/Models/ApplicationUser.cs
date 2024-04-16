using Microsoft.AspNetCore.Identity;
using Stripe;

namespace StripeItegration.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string SubscriptionLevel { get; set; } = "none";
        public string Role { get; set; } = "DefaultUser";
    }
}
