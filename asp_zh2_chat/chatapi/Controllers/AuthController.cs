using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using chatapi.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace chatapi.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> users, IConfiguration config)
        {
            this._userManager = users;
            this._configuration = config;
        }

        [Route("/register")]
        [HttpPost]
        public async Task<ActionResult> InsertUser([FromBody] RegisterViewModel model)
        {
            var user = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Username,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await this._userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return Unauthorized($"Registration failed. {string.Join(" ", result.Errors.Select(x => x.Description))}");
            
            return Ok(new { email = user.Email, username = user.UserName });
        }

        [Route("/login")]
        [HttpPost]
        public async Task<ActionResult> Login([FromBody] LoginViewModel model)
        {
            var user = await this._userManager.FindByEmailAsync(model.Email);
            if (user != null && await this._userManager.CheckPasswordAsync(user, model.Password))
            {
                var claim = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email)
                };
                var signinKey = new SymmetricSecurityKey(
                  Encoding.UTF8.GetBytes(this._configuration["Jwt:SigningKey"]));

                int expiryInMinutes = Convert.ToInt32(this._configuration["Jwt:ExpiryInMinutes"]);

                var token = new JwtSecurityToken(
                  issuer: this._configuration["Jwt:Site"],
                  audience: this._configuration["Jwt:Site"],
                  claims: claim,
                  expires: DateTime.Now.AddMinutes(60),
                  signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(
                  new
                  {
                      token = new JwtSecurityTokenHandler().WriteToken(token),
                      expiration = token.ValidTo
                  });
            }

            return Unauthorized("Login failed. Wrong credentials.");
        }
    }

}