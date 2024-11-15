using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMeals.Server.Data;
using SchoolMeals.Server.Interfices;
using SchoolMeals.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SchoolMeals.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IHelpers _help;
        private readonly UserManager<Users> _userManager;

        // Date
        private readonly DateTime _date;

        public UsersController(
                    AppDbContext db,
                    IHelpers help,
                    UserManager<Users> userManager
            )
        {
            _db = db;
            _help = help;
            _userManager = userManager;
            _date = DateTime.Now;
        }

        #region GET
        [HttpGet("GetCurrent")]
        public async Task<ActionResult<Users>> GetCurrentUser()
        {
            Users user = await GetAuthenticatedUser();
            return await Task.FromResult(user);
        }

        [HttpGet("GetUsersSchool/{email}")]
        public async Task<ActionResult<School>> GetCurrentUsersSchool(string email)
        {
            //if (!User.Identity.IsAuthenticated)
            //    return null;

            School school = null;
            var user = _db.Users.Include(s => s.School).FirstOrDefault(x => x.Email == email);
            if (user.School != null)
                school = user.School;

            return await Task.FromResult(school);
        }

        [HttpGet("GetUserRoles/{email}")]
        [Authorize]
        public async Task<Object> GetRoles(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var roles = _userManager.GetRolesAsync(user).Result;
            return await Task.FromResult(roles);
        }

        #endregion


        #region Helpers
        public async Task<Users> GetAuthenticatedUser()
        {
            Users user = new Users();
            if (User.Identity.IsAuthenticated)
            {
                //user.Email = User.FindFirstValue(ClaimTypes.Email);
                user = await _userManager.FindByEmailAsync(User.Identity.Name);
            }

            return user;
        }
        #endregion
    }
}
