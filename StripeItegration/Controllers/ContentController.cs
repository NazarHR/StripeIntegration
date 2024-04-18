using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetBaseSubscriptionContetn()
        {
            return Ok("this contetnt is for members with base subscription");
        }

        [HttpGet("/platinum")]
        public IActionResult GetPalinumSubscriptionContetn()
        {
            return Ok("this contetnt is for members with base subscription");
        }

        [HttpGet("/diamond")]
        public IActionResult GetDiamondSubscriptionContetn()
        {
            return Ok("this contetnt is for members with base subscription");
        }
    }
}
