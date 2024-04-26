using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using StripeItegration.AuthorizeAttributes;
using StripeItegration.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace StripeIntegration.Test
{
    public class SubscriptionRequiredAttributeTests
    {
        [Fact]
        public void SubscriptionRequiredAttribute_UserIsNull_ReturnsUnauthorizedResult()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.User = null;
            var actionContext = new ActionContext(httpContext,
                                    new Microsoft.AspNetCore.Routing.RouteData(),
                                    new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
            var autorizartionFilterContext = new AuthorizationFilterContext(
                actionContext, Array.Empty<IFilterMetadata>());
            
            var subscriptionRequriedAttribute = new SubscriptionRequriedAttribute();

            //Act
            subscriptionRequriedAttribute.OnAuthorization(autorizartionFilterContext);

            //Assert
            Assert.IsType<UnauthorizedResult>(autorizartionFilterContext.Result);
        }

        [Fact]
        public void SubscriptionRequiredAttribute_UserIsNotAuthenticated_ReturnsUnauthorizedResult()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var identityMock = new Mock<IIdentity>();
            identityMock.SetupGet(x=>x.IsAuthenticated).Returns(false);
            var userMock = new Mock<ClaimsPrincipal>();
            userMock.SetupGet(x => x.Identity).Returns(identityMock.Object);
            httpContext.User = userMock.Object;
            var actionContext = new ActionContext(httpContext,
                                    new Microsoft.AspNetCore.Routing.RouteData(),
                                    new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
            var autorizartionFilterContext = new AuthorizationFilterContext(
                actionContext, Array.Empty<IFilterMetadata>());

            var subscriptionRequriedAttribute = new SubscriptionRequriedAttribute();

            //Act
            subscriptionRequriedAttribute.OnAuthorization(autorizartionFilterContext);

            //Assert
            Assert.IsType<UnauthorizedResult>(autorizartionFilterContext.Result);
        }
        [Fact]
        public void SubscriptionRequiredAttribute_UserDoNotHaveSubscription_ReturnsForbidResult()
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var identityMock = new Mock<ClaimsIdentity>();
            identityMock.SetupGet(x => x.IsAuthenticated).Returns(true);
            var userMock = new Mock<ClaimsPrincipal>();
            userMock.SetupGet(x => x.Identity).Returns(identityMock.Object);
            httpContext.User = userMock.Object;
            var actionContext = new ActionContext(httpContext,
                                    new Microsoft.AspNetCore.Routing.RouteData(),
                                    new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
            var autorizartionFilterContext = new AuthorizationFilterContext(
                actionContext, Array.Empty<IFilterMetadata>());

            var subscriptionRequriedAttribute = new SubscriptionRequriedAttribute();

            //Act
            subscriptionRequriedAttribute.OnAuthorization(autorizartionFilterContext);

            //Assert
            Assert.IsType<ForbidResult>(autorizartionFilterContext.Result);
        }
        [Theory]
        [InlineData("Diamond", "Platinum")]
        [InlineData("Diamond", "Base")]
        [InlineData("Platinum", "Base")]
        public void SubscriptionRequiredAttribute_UsersSubscriptionLevelIsLowerThanRequired_ReturnsForbidResult(
            string requiredSubscriptionLevel, string testUserSubscriptionLevel)
        {
            //Arrange
            var httpContext = new DefaultHttpContext();
            var identityMock = new Mock<ClaimsIdentity>();
            identityMock.SetupGet(x => x.IsAuthenticated).Returns(true);
            identityMock.Setup(x => x.FindFirst(It.Is<string>(x => x == "SubscriptionLevel")))
                .Returns(new Claim("SubscriptionLevel", testUserSubscriptionLevel));
            var userMock = new Mock<ClaimsPrincipal>();
            userMock.SetupGet(x => x.Identity).Returns(identityMock.Object);
            httpContext.User = userMock.Object;
            var actionContext = new ActionContext(httpContext,
                                    new Microsoft.AspNetCore.Routing.RouteData(),
                                    new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
            var autorizartionFilterContext = new AuthorizationFilterContext(
                actionContext, Array.Empty<IFilterMetadata>());

            var subscriptionRequriedAttribute = new SubscriptionRequriedAttribute()
            {
                Level = requiredSubscriptionLevel
            };

            //Act
            subscriptionRequriedAttribute.OnAuthorization(autorizartionFilterContext);

            //Assert
            Assert.IsType<ForbidResult>(autorizartionFilterContext.Result);
        }
    }
}
