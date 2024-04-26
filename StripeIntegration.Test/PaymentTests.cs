using Microsoft.AspNetCore.Identity;
using Moq;
using StripeItegration.Controllers;
using StripeItegration.Entities;

namespace StripeIntegration.Test
{
    public class PaymentTests
    {
        [Fact]
        public void CreatePaymentLink_UnexistingProduct_ReturnsBadRequest()
        {
            var userManagerMock = new Mock<UserManager<ApplicationUser>>();

            var paymentController = new PaymentController(userManagerMock.Object);
        }
    }
}