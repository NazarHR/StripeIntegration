using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StripeItegration.Entities;
using System.Security.Claims;
using System.Web.Mvc.Filters;

namespace StripeItegration.AuthorizeAttributes
{

    public class SubscriptionRequriedAttribute : Attribute, IAuthorizationFilter
    {
        public string Level { get; set; } = "Base";

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var identity = (ClaimsIdentity)user.Identity;
            var currentLevelClaim = identity.FindFirst("SubscriptionLevel");
            var currentLevelString = "None";
            if (currentLevelClaim != null)
            {
                currentLevelString = currentLevelClaim.Value;
            }

            var currentLevel = Enum.Parse<SubscriptionLevel>(currentLevelString);
            var expectedLevel = Enum.Parse<SubscriptionLevel>(Level);
            if(currentLevel < expectedLevel)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
    enum SubscriptionLevel
    {
        None = 0,
        Base = 1,
        Platinum = 2,
        Diamond = 3
    }
}
