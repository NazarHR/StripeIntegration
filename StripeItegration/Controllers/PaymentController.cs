using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using System.Web;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Authorization;
using StripeItegration.Entities;
using Microsoft.AspNetCore.Identity;
using StripeItegration.Models;

namespace StripeItegration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        //test credit cards
        //4242 4242 4242 4242 - success
        //4000 0027 6000 3184 - authentication needed
        //4000 0000 0000 0002 - failed
        [HttpPost]
        public IActionResult Create(PaymentSessionExternalParametersModel paymentParameters)
        {

            var priceOptions = new PriceListOptions
            {
                Product = paymentParameters.Product_Id,
            };
            var priceService = new PriceService();
            StripeList<Price> prices;
            try
            {
                prices = priceService.List(priceOptions);
            }
            catch(StripeException ex)
            {

                return BadRequest(ex.Message);
            }
            
            var options = new SessionCreateOptions
            {
                //UiMode = "embedded",
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    Price = prices.Data[0].Id,
                    Quantity = 1,
                  },
                },
                ClientReferenceId = HttpContext.User.Identity.Name,
                Mode = "subscription",
                SuccessUrl = paymentParameters.SuccessReturnUrl,
            };
            var service = new SessionService();
            Session session = service.Create(options);
            Console.WriteLine(session.ReturnUrl);
            return Ok(session.ToJson());
        }
        [AllowAnonymous]
        [HttpGet("/success", Name = "Success")]
        public async Task<IActionResult> OrderSuccess()
        {            
            return Ok("Subscription successful");
        }
        [AllowAnonymous]
        [HttpGet("/cancel", Name = "Cancel")]
        public IActionResult Cancel()
        {
            return BadRequest();
        }
        [HttpPatch("/resume/{subscription_item_id}")]
        public IActionResult ResumeSubscription(string subscription_item_id)
        {
            var options = new SubscriptionResumeOptions
            {
                BillingCycleAnchor = SubscriptionBillingCycleAnchor.Now,
            };
            var service = new SubscriptionService();
            service.Resume(subscription_item_id, options);
            return Ok("Subscription resumed");
        }

        [HttpDelete("/delete/{subscription_item_id}")]
        public IActionResult CancelSubscription(string subscription_item_id)
        {
            var service = new SubscriptionService();
            service.Cancel(subscription_item_id);
            return Ok("Subscription cancelled");
        }
    }
}
