using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace StripeItegration.Controllers
{
    [Route("webhook")]
    [ApiController]
    public class WebhookController : Controller
    {
        private readonly string endpointSecret;
        private readonly IConfiguration _configuration;

        public WebhookController(IConfiguration configuration)
        {
            _configuration = configuration;
            endpointSecret = configuration["Stripe:WebHookSecret"];
        }
        [HttpPost]
        public async Task<IActionResult> EventListenerAsync()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);

                // Handle the event
                if (stripeEvent.Type == Events.CheckoutSessionAsyncPaymentSucceeded)
                {
                    await Console.Out.WriteLineAsync("Succesful checkout");
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
