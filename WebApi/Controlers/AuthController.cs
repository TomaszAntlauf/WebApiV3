using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApi.Models;

namespace WebApi.Controlers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private DatabaseContext context { get; set; }

        public AuthController(DatabaseContext con)
        {
            context = con;
        }

        /// <summary>
        /// Umożliwia zalogowanie się użytkownikowi
        /// </summary>
        [HttpPost, Route("login")]
        public IActionResult Login([FromBody]Login user)
        {
            if (user == null)
            {
                return BadRequest("Invalid client request");
            }

            if (context.User.Any(m => m.Username == user.Username && m.Password == user.Password))
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, context.User.Where(u => user.Username == u.Username ).Single().Typ)
                };

                var tokeOptions = new JwtSecurityToken(
                    issuer: "http://localhost:44372",
                    audience: "http://localhost:44372",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new { Token = tokenString, uType = context.User.Where(u => user.Username == u.Username).Single().Typ });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}