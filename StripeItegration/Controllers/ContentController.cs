using Microsoft.AspNetCore.Mvc;
using StripeItegration.AuthorizeAttributes;

namespace StripeItegration.Controllers
{
    [ApiController]
    [Route("/content")]
    public class ContentController : Controller
    {
        [HttpGet("/free")]
        public IActionResult FreeContent()
        {
            return Ok("this content is free");
        }
        
        [HttpGet("/base")]
        [SubscriptionRequried]
        public IActionResult GetBaseSubscriptionContetn()
        {
            return Ok("this contetnt is for members with base subscription");
        }
        
        [HttpGet("/platinum")]
        [SubscriptionRequried(Level = "Platinum")]
        public IActionResult GetPalinumSubscriptionContetn()
        {
            return Ok("this contetnt is for members with base subscription");
        }

        [HttpGet("/diamond")]
        [SubscriptionRequried(Level = "Diamond")]
        public IActionResult GetDiamondSubscriptionContetn()
        {
            return Ok("this contetnt is for members with base subscription");
        }
    }
}
