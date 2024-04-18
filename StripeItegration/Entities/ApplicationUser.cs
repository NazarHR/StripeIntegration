using Microsoft.AspNetCore.Identity;
using Stripe;

namespace StripeItegration.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? SubscriptionLevel { get; set; }
        public string? StripeUserId { get; set; }
    }
}
