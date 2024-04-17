using Microsoft.AspNetCore.Identity;
using Stripe;

namespace StripeItegration.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? SubscriptionLevel { get; set; }
        public string? StripeUserId { get; set; }
    }
}
