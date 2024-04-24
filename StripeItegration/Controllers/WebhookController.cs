using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using StripeItegration.Entities;
using System.Text.Json;

namespace StripeItegration.Controllers
{
    [Route("webhook")]
    [ApiController]
    public class WebhookController : Controller
    {
        private readonly string endpointSecret;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public WebhookController(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            endpointSecret = configuration["Stripe:WebHookSecret"];
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> EventListenerAsync([FromBody]JsonElement stripeEventJson)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(stripeEventJson.ToString(),
                    Request.Headers["Stripe-Signature"], endpointSecret);

                // Handle the event
                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var sessionService = new SessionService();
                    var session = (Session)stripeEvent.Data.Object;

                    var user = await _userManager.FindByNameAsync(session.ClientReferenceId);

                    var products = await sessionService.ListLineItemsAsync(session.Id);
                    var product = products.First();
                    var subscriptinoPlan = product.Description.Split(" ")[0];

                    user.SubscriptionLevel = subscriptinoPlan;

                    user.StripeUserId = session.CustomerId;
                    await _userManager.UpdateAsync(user);
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionUpdated)
                {
                    var subscription = (Subscription)stripeEvent.Data.Object;
                    var subscriptionItemService = new SubscriptionItemService();
                    var item = subscription.Items.Data[0];
                    var service = new ProductService();
                    var productaName = service.Get(item.Plan.ProductId).Description;
                    var user = _userManager.Users.First(u => u.StripeUserId == subscription.CustomerId);
                    user.SubscriptionLevel = productaName.Split(" ")[0];
                    await _userManager.UpdateAsync(user);

                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionDeleted)
                {
                    var subscription = (Subscription)stripeEvent.Data.Object;
                    var user = _userManager.Users.First(u => u.StripeUserId == subscription.CustomerId);
                    user.SubscriptionLevel = "none";
                    await _userManager.UpdateAsync(user);
                }
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
    }
}
