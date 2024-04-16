using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using System.Web;
using static System.Net.WebRequestMethods;

namespace StripeItegration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : Controller
    {
        //test credit cards
        //4242 4242 4242 4242 - success
        //4000 0027 6000 3184 - authentication needed
        //4000 0000 0000 0002 - failed
        [HttpPost]
        public IActionResult Create(string prodict_id)
        {
            string domain = string.Format("{0}://{1}",
                       HttpContext.Request.Scheme, HttpContext.Request.Host);

            var priceOptions = new PriceListOptions
            {
                Product = prodict_id
            };
            var priceService = new PriceService();
            StripeList<Price> prices = priceService.List(priceOptions);
            
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    Price = prices.Data[0].Id,
                    Quantity = 1,
                  },
                },
                Mode = "subscription",
                SuccessUrl = domain + "/success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = Url.Link("Cancel",new { }),
            };
            var service = new SessionService();
            Session session = service.Create(options);
            return Ok(session.Url);
        }

        [HttpGet("/success", Name = "Success")]
        public ActionResult OrderSuccess([FromQuery]string session_id)
        {
            var sessionService = new SessionService();
            Session session = sessionService.Get(session_id);

            var customerService = new CustomerService();
            Customer customer = customerService.Get(session.CustomerId);

            return Created(session_id, customer);
        }
        [HttpGet("/cancel", Name = "Cancel")]
        public IActionResult Cancel()
        {
            return BadRequest();
        }

        [HttpPatch("/pause/{subscription_item_id}")]
        public IActionResult PauseSubscription(string subscription_item_id)
        {
            throw new NotImplementedException();
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
            service.Cancel("subscription_item_id");
            return Ok("Subscription cancelled");
        }
    }
}
