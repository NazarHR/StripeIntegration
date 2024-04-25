using Microsoft.AspNetCore.Mvc;
using StripeItegration.AuthorizeAttributes;

namespace StripeItegration.Controllers
{
    [ApiController]
    [Route("/content")]
    public class ContentController : Controller
    {
        /// <summary>
        /// Get free content
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/content/free
        ///     
        /// Sample Response:
        /// 
        ///     {        
        ///       "Message" = "this content is free"
        ///     }
        /// </remarks>
        /// <response code="200">Message</response>
        [HttpGet("/free")]
        public IActionResult FreeContent()
        {
            return Ok(
                new {
                    message = "this content is free" 
                });
        }
        /// <summary>
        /// Get content accessible with any level of subscription
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/content/base
        ///     
        /// Sample Response:
        /// 
        ///     {        
        ///       "Message" = "this contetnt is for members with base subscription" 
        ///     }
        /// </remarks>
        /// <response code="200">Message</response>
        /// <response code="403">If user do not have required level of subscription</response>
        [HttpGet("/base")]
        [SubscriptionRequried]
        public IActionResult GetBaseSubscriptionContetn()
        {
            return Ok(
                new { 
                    Message = "this contetnt is for members with base subscription" 
                });
        }
        /// <summary>
        /// Get content accessible with platinum level of subscription or higher
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/content/platinum
        ///     
        /// Sample Response:
        /// 
        ///     {        
        ///       "Message" = "this contetnt is for members with paltinum subscription" 
        ///     }
        /// </remarks>
        /// <response code="200">Message</response>
        /// <response code="403">If user do not have required level of subscription</response>
        [HttpGet("/platinum")]
        [SubscriptionRequried(Level = "Platinum")]
        public IActionResult GetPalinumSubscriptionContetn()
        {
            return Ok(
                new { 
                    Message = "this contetnt is for members with paltinum subscription" 
                });
        }
        /// <summary>
        /// Get content accessible with diamond level of subscription or higher
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/content/diamond
        ///     
        /// Sample Response:
        /// 
        ///     {        
        ///       "Message" = "this contetnt is for members with diamond subscription"
        ///     }
        /// </remarks>
        /// <response code="200">Message</response>
        /// <response code="403">If user do not have required level of subscription</response>
        [HttpGet("/diamond")]
        [SubscriptionRequried(Level = "Diamond")]
        public IActionResult GetDiamondSubscriptionContetn()
        {
            return Ok(
                new
                {
                    Message = "this contetnt is for members with diamond subscription"
                });
        }
    }
}
