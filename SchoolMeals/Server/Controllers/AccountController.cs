using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SchoolMeals.Server.Data;
using SchoolMeals.Server.Interfices;
using SchoolMeals.Shared.Models;
using SchoolMeals.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolMeals.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<Users> _signInManager;
        private readonly UserManager<Users> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHelpers _help;
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AccountController(
            UserManager<Users> userManager,
            SignInManager<Users> signInManager,
            ILogger<AccountController> logger,
            RoleManager<IdentityRole> roleManager,
            IConfiguration config,
            IHelpers help,
            AppDbContext db
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _help = help;
            _db = db;
            _config = config;
        }


        #region POST
        // Register
        [HttpPost("Register")]
        public async Task<IActionResult> OnPostAsync(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var admin_email = model.Email.Equals("aslan_argun@hotmail.com");
                var user = new Users { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.UpdateAsync(user);
                    List<string> roles = new List<string> { "User" };
                    if (admin_email)
                    {
                        roles.Add("Admin");
                        roles.Add("Support");
                    }

                    for (var i = 0; i < roles.Count(); i++)
                    {
                        var role = _roleManager.FindByNameAsync(roles[i]).Result;
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    var token = GenerateJwtToken(user, roles);

                    return Ok(new { secureToken = token, name = user.FirstName });
                }
                var error_msg = "";
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    error_msg += error.Description + " ";
                }
                return BadRequest(error_msg);
            }

            return BadRequest("Formuläret är inte korrekt ifyllt");
        }

        // Logga in
        [HttpPost("Login")]
        public async Task<IActionResult> OnPostLogin([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.Remember, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Email);
                    var roles = _userManager.GetRolesAsync(user).Result;

                    var days = (model.Remember) ? 7 : 1;
                    //var claim = new Claim(ClaimTypes.Name, model.Email);

                    var token = GenerateJwtToken(user, roles.ToList(), days);

                    return Ok(new { secureToken = token, name = user.FirstName });
                }

                if (result.IsLockedOut)
                    return BadRequest("Användarkonto låst ut");
                else
                    return BadRequest("Felaktig e-post eller lösenord");

            }

            return BadRequest("Formuläret är inte korrekt ifyllt");
        }

        // Send new password
        //[HttpPost("ChangePassword")]
        //[Authorize]
        //public async Task<Object> PostPassForgot(ChangePasswordViewModel model)
        //{
        //    var user = await _userManager.FindByEmailAsync(model.Email);
        //    if (user != null)
        //    {
        //        var pass = Guid.NewGuid().ToString().Substring(0, 10);
        //        var remove = await _userManager.RemovePasswordAsync(user);
        //        if (remove.Succeeded)
        //        {
        //            // If old pass removed, insert new pass
        //            var password = await _userManager.AddPasswordAsync(user, pass);

        //            if (password.Succeeded)
        //            {
        //                var content = "<h3>Nytt lösenord</h3><br/>" +
        //                              "<p>Ditt lösenord har ändrats.</p>" +
        //                              "<p>Ditt nytt lösenord: <span style='font-weight:bold;margin-lleft:15px'>" + pass + "</span></p>";

        //                // Email
        //                _help.SendMail("Email", user.Email, "Nytt lösenord", content, GetDomain());
        //                return new JsonResult(new { success = true, msg = "Nytt lösenord är skickad, kontrollera din e-post" });
        //            }
        //            else
        //                return new JsonResult(new { warning = true, msg = "Det gick inte spara nytt lösenord, var vänlig försök igen senare ..." });
        //        }
        //    }
        //    return new JsonResult(new { error = true, msg = "Användare med matchande Email har inte hittats ..." });
        //}
        #endregion

        #region GET
        // Logg out
        [HttpGet("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Användare loggad ut.");
            return Ok();
        }
        #endregion

        #region Helpers
        // Generate JWT Token
        private string GenerateJwtToken(Users user, List<string> roles, int days = 1)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            IdentityOptions opt = new IdentityOptions();

            var claim = new List<Claim>();
            claim.Add(new Claim("Id", user.Id.ToString()));
            claim.Add(new Claim("Email", user.Email));
            claim.Add(new Claim("ApiKey", "SchoolMealsKey"));
            foreach (var r in roles)
                claim.Add(new Claim(opt.ClaimsIdentity.RoleClaimType, r));


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claim.ToArray()),
                Expires = DateTime.Now.AddDays(days),
                SigningCredentials = credentials
            };

            var encodeToken = new JwtSecurityTokenHandler();
            var securityToken = encodeToken.CreateToken(tokenDescriptor);
            var token = encodeToken.WriteToken(securityToken);

            return token;
        }
        #endregion
    }
}
            //// Set claims i claimsprincipial ----
            //var identity = new ClaimsIdentity(claim.ToArray());
            //var claimsPrincipal = new ClaimsPrincipal(identity);
            //// Set current principal ----
            //Thread.CurrentPrincipal = claimsPrincipal;
            //var idsidentity = (ClaimsPrincipal)Thread.CurrentPrincipal;