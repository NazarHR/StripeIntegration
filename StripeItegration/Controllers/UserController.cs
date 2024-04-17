using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StripeItegration.DbContext;
using StripeItegration.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StripeItegration.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public UserController(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Get an authentication token to use for calling the endpoints
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/authentication/token
        ///     {        
        ///       "username": "john",
        ///       "password": "John@123#"   
        ///     }
        /// Sample Response:
        /// 
        ///     {        
        ///       "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c",
        ///       "expiration": "2022/08/23 12:03:03"   
        ///     }
        /// </remarks>
        /// <response code="200">Authentication Token and Expiration Time</response>
        /// <response code="401">If crednetials are wrong</response>  
        [HttpPost("/token")]
        public async Task<IActionResult> Token([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Sid, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }
        [HttpPost("/register", Name = "Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            if (_userManager.FindByNameAsync(registerModel.Username).Result == null)
            {
                var user = new ApplicationUser();
                user.UserName = registerModel.Username;
                user.Email = registerModel.Email;

                var result = await _userManager.CreateAsync(user, registerModel.Password);
                if(result.Succeeded)
                {
                    return Ok();
                }
                return BadRequest(result.Errors);
            }
            return BadRequest( IdentityResult.Failed(
                new IdentityError
                {
                    Description = "User Already Exist"
                }));
        }
    }
}
