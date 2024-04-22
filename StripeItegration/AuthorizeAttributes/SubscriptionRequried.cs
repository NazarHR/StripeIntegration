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
        public string? Level { get; set; } = "Base";

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var identity = (ClaimsIdentity)user.Identity;
            var currentLevel = identity.FindFirst("SubscriptionLevel").Value;
            if(currentLevel == null) 
            { 
                context.Result = new UnauthorizedResult();
                return;
            }
            if(currentLevel != Level)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
