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


        /// <summary>
        /// Get a url for checkout page
        /// </summary>
        /// <remarks>
        /// 
        /// Test credit cards:
        ///     4242 4242 4242 4242 - success
        ///     4000 0027 6000 3184 - authentication needed
        ///     4000 0000 0000 0002 - failed
        /// Sample request:
        /// 
        ///     Diamond
        ///     POST api/Payment 
        ///     {
        ///         "product_Id": "prod_PvbgXODqDeAM85",
        ///         "successReturnUrl": "https://localhost:7035/success"
        ///     }
        ///     
        ///     Platinum
        ///     POST api/Payment 
        ///     {
        ///         "product_Id": "prod_Pvbf0OGhSDvtGK",
        ///         "successReturnUrl": "https://localhost:7035/success"
        ///     }
        ///     
        ///     Base
        ///     POST api/Payment 
        ///     {
        ///         "product_Id": "prod_Pvb3B4isOtoUTm",
        ///         "successReturnUrl": "https://localhost:7035/success"
        ///     }
        ///     
        /// Sample Response:
        /// 
        ///     {  
        ///         "username" = "Admin",
        ///         "CheckoutUrl" = "https://checkout.stripe.com/c/pay/cs_test_a1uVz1oNzCCNDx3Y2V9b3JLEAAWKI49enqvSVhN1D4hFDqlK2o1AX5xQjY#fidkdWxOYHwnPyd1blpxYHZxWjA0VTFUUGpCbmk3aj01VEpVSlJLdm08cn91fWpnYFxIdUR3aGxVYGNVdjR1V25DMG1DbUBBUTBhZkdpVF1GXXVNZDdjdkNKMG9Kc0FIQU9hPEM8YkFMXE81NTVKcDc9bXF9cycpJ2N3amhWYHdzYHcnP3F3cGApJ2lkfGpwcVF8dWAnPyd2bGtiaWBabHFgaCcpJ2BrZGdpYFVpZGZgbWppYWB3dic%2FcXdwYHgl"
        ///     }
        /// </remarks>
        /// <response code="200">User Name and Checkout Url</response>
        /// <response code="400">Error message</response>
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
            return Ok(
                new
                {
                    username = User.Identity.Name,
                    CheckoutUrl = session.Url
                });
        }
        /// <summary>
        /// Plug route
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     GET api/success 
        /// </remarks>
        /// <response code="200"></response>
        [AllowAnonymous]
        [HttpGet("/success", Name = "Success")]
        public async Task<IActionResult> OrderSuccess()
        {            
            return Ok();
        }
        /// <summary>
        /// Resume paused subscription
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Patch api/resume/{subscription_id}
        ///     
        /// Sample Response:
        /// 
        ///     {        
        ///       "Id": "Si_Ty48KfosU",
        ///       "Status": "Active"
        ///     }
        /// </remarks>
        /// <response code="200">Id and Status</response>
        /// <response code="403">If user do not own the subscription</response>
        /// <response code="404">If subscription do not exist</response>
        [HttpPatch("/resume/{subscription_item_id}")]
        public async Task<IActionResult> ResumeSubscriptionAsync(string subscription_item_id)
        {

            var service = new SubscriptionService();
            Subscription subscription;
            try
            {
                subscription = service.Get(subscription_item_id);
            }
            catch(StripeException ex)
            {
                return NotFound(ex.Message);
            }
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if(subscription.CustomerId != currentUser.StripeUserId)
            {
                return Forbid("You dont own this subscription");
            }
            var options = new SubscriptionResumeOptions
            {
                BillingCycleAnchor = SubscriptionBillingCycleAnchor.Now,
            };
            var resultSubscription =  service.Resume(subscription_item_id, options);
            return Ok(
                new
                {
                    Id = resultSubscription.Id,
                    Status = resultSubscription.Status
                });
        }
        /// <summary>
        /// Delete subscription
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Patch api/delete/{subscription_id}
        ///     
        /// Sample Response:
        /// 
        ///     {        
        ///       "Message" = "Subscription cancelled"
        ///     }
        /// </remarks>
        /// <response code="200">Message</response>
        /// <response code="403">If user do not own the subscription</response>
        /// <response code="404">If subscription do not exist</response>
        [HttpDelete("/delete/{subscription_item_id}")]
        public async Task<IActionResult> CancelSubscriptionAsync(string subscription_item_id)
        {
            var service = new SubscriptionService();
            Subscription subscription;
            try
            {
                subscription = service.Get(subscription_item_id);
            }
            catch (StripeException ex)
            {
                return NotFound(ex.Message);
            }
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (subscription.CustomerId != currentUser.StripeUserId)
            {
                return Forbid("You do not own this subscription");
            }
            service.Cancel(subscription_item_id);
            return Ok(
                new { 
                    Message = "Subscription cancelled" 
                });
        }
    }
}
