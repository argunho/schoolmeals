using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMeals.Server.Data;
using SchoolMeals.Server.Interfices;
using SchoolMeals.Shared.Models;
using SchoolMeals.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolMeals.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MealController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _host;
        private readonly IHelpers _help;

        // Gets the Calendar instance associated with a CultureInfo.
        private readonly CultureInfo _ci;
        private readonly Calendar _cal;

        // Gets the DTFI properties required by GetWeekOfYear.
        private readonly CalendarWeekRule _cwr;
        private readonly DayOfWeek _dow;

        // Date
        private readonly DateTime _date;

        public MealController(
                    AppDbContext db,
                    IHelpers help,
                    IWebHostEnvironment host
            )
        {
            _db = db;
            _help = help;
            _host = host;
            _ci = new CultureInfo("sv-SE");
            _cal = _ci.Calendar;
            _cwr = _ci.DateTimeFormat.CalendarWeekRule;
            _dow = _ci.DateTimeFormat.FirstDayOfWeek;
            _date = DateTime.Now;
        }

        private IEnumerable<Meal> AllMeal
        {
            get
            {
                return _db.Meal.Include(s => s.School).ToList();
            }
        }


        #region GET
        // GET: All Meal by school id for support
        [HttpGet("{id}")]
        public Meal Get(int id)
        {
            return _db.Meal.Include(s => s.School).FirstOrDefault(x => x.Id == id);
        }

        // GET: All Meal by school id for support
        [HttpGet("GetBySchoolId/{id}")]
        [Authorize(Roles = "Admin, Support, Customer")]
        public IEnumerable<Meal> GetMealBySchool(int id)
        {
            return AllMeal.Where(x => x.School.Id == id).ToList();
        }

        // GET: All Meal by school and current week id for support
        [HttpGet("GetBySchoolIdAndCurrentWeek/{id}")]
        public IEnumerable<Meal> GetMealBySchoolAndCurrentWeek(int id)
        {
            var week = ReturnWeekNumber(_date);
            var list = AllMeal.Where(x => x.School.Id == id && x.Week == week && x.Date.Year == _date.Year).OrderBy(x => x.Date).ToList();
            return list;
        }

        // GET: All Meal by school and week
        [HttpGet("GetBySchoolAndWeek/{id}/date/{date}")]
        public IEnumerable<Meal> GetMealBySchoolAndWeek(int id, string date)
        {

            DateTime _date = Convert.ToDateTime(date);
            var week = ReturnWeekNumber(_date);
            var list = AllMeal.Where(x => x.School.Id == id && x.Week == week && x.Date.Year == _date.Year).OrderBy(x => x.Date).ToList();
            return list;
        }

        // GET: All Meal by school
        [HttpGet("GetMealsBySchoolAndDate/{id}/{date}")]
        public IEnumerable<Meal> GetWeeksMealByDate(int id, string date)
        {
            DateTime _date = Convert.ToDateTime(date);
            return AllMeal.Where(x => x.School.Id == id && x.Date.Year == _date.Year && x.Date.Month == _date.Month && x.Date.Day == _date.Day);
        }

        // Get by date
        [HttpGet("GetBySchoolId/{id}/week/{week}/year/{year}")]
        public IEnumerable<Meal> GetMealBySchoolAndWeek(int id, int week, int year)
        {
            return AllMeal.Where(x => x.School.Id == id && x.Week == week && x.Date.Year == year).OrderBy(x => x.Date).ToList();
        }

        // Get Meal by id (support or customer access)
        [HttpGet("GetBySchoolIdAndMealId/{schoolId}/{mealId}")]
        //[Authorize(Roles = "Admin, Support")]
        public Meal GetMealBySchoolAndId(int schoolId, int mealId)
        {
            return AllMeal.FirstOrDefault(x => x.Id == mealId && x.School.Open && x.School.Id == schoolId);
        }

        [HttpGet("weekByDate/{date}")]
        public string GetWeek(string date)
        {
            DateTime _date = Convert.ToDateTime(date);
            return ReturnWeekNumber(_date).ToString();
        }
        #endregion

        #region POST
        // POST New Meal
        [HttpPost]
        //[Authorize(Roles = "Admin, Support")]
        public async Task<IActionResult> Post(MealRecipeGenericViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string imgName = "";
                    dynamic upload = false;

                    if (model.File != null)
                    {
                        // Set new name for image
                        imgName = _help.ImgName("meals/" + model.Name.Replace(" ", "_"), model.FileName);
                        // Save image
                        upload = _help.SaveFile(model.File, imgName);
                    }

                    var school = _db.School.FirstOrDefault(x => x.Id == model.SchoolId);
                    if (school == null)
                        return BadRequest("Skola med matchande Id kunde inte hittats ...");

                    var existsDate = AllMeal.FirstOrDefault(x => x.Date.ToString("yyyy.MM.dd") == model.Date.ToString("yyyy.MM.dd") && x.School.Id == model.SchoolId) != null;
                    if (existsDate)
                        return BadRequest("Det finns redan en måltid med samma datum ...");

                    var Meal = new Meal
                    {
                        Name = model.Name,
                        Text = model.Text,
                        Ingredients = model.Ingredients,
                        School = school,
                        RecipeId = model.RecipeId,
                        Date = model.Date,
                        Week = ReturnWeekNumber(model.Date),
                        DayOfWeek = ReturnDayOfWeek(model.Date),
                        ImgUrl = (upload is bool && upload) ? imgName : (model.ImgUrl != null && model.RecipeId > 0) ? model.ImgUrl : ""
                    };
                    _db.Meal.Add(Meal);
                    await _db.SaveChangesAsync();


                    if (upload is string)
                        return Ok("Data skickades framgångsrikt men bilden kunde inte sparas!=>" + upload);

                    return Ok("Data skickades framgångsrikt!");
                }
                catch (Exception e)
                {
                    return BadRequest("Något har gått snett, var vänlig försök senare ... <br/>Error => " + e.Message);
                }
            }

            return BadRequest("Formulär var inte korrekt ifyllt ... ");
        }
        #endregion

        #region PUT
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin, Support, Customer")]
        public async Task<IActionResult> Put(int id, MealRecipeGenericViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string imgName = "";
                    string imgToRemove = "";
                    dynamic upload = false;

                    if (model.File != null)
                    {
                        // Set new name for image
                        imgName = _help.ImgName("meals/" + model.Name, model.FileName);
                        // Save image
                        upload = _help.SaveFile(model.File, imgName);
                    }

                    var meal = AllMeal.FirstOrDefault(x => x.Id == id);
                    if (meal == null)
                        return BadRequest("Måltid med matchande Id kunde inte hittats");

                    var existsDate = AllMeal.FirstOrDefault(x => x.Id != meal.Id && x.Date.ToString("yyyy.MM.dd") == model.Date.ToString("yyyy.MM.dd") && x.School.Id == model.SchoolId) != null;
                    if (existsDate)
                        return BadRequest("Det finns redan en måltid med samma datum ...");

                    meal.Name = model.Name;
                    meal.Ingredients = model.Ingredients;
                    meal.Text = model.Text;
                    meal.DayOfWeek = ReturnDayOfWeek(model.Date);
                    meal.Week = ReturnWeekNumber(model.Date);
                    meal.Date = model.Date;
                    if (upload is bool && upload)
                    {
                        imgToRemove = meal.ImgUrl;
                        meal.ImgUrl = imgName;
                    }
                    else if (model.RecipeId != meal.RecipeId && model.ImgUrl != null)
                    {
                        meal.ImgUrl = model.ImgUrl;
                    }
                    meal.RecipeId = model.RecipeId;

                    await _db.SaveChangesAsync();
                    if (upload is bool && upload)
                        _help.DeleteImage(imgToRemove);

                    if (upload is string)
                        return Ok("Data skickades framgångsrikt men bilden kunde inte sparas. <br/> Errorn => " + upload);

                    return Ok("Data skickades framgångsrikt!");
                }
                catch (Exception e)
                {
                    return BadRequest("Något har gått snett, var vänlig försök senare ...  <br/> Errorn => " + e.Message);
                }
            }

            return BadRequest("Formulär var inte korrekt ifyllt ... ");
        }

        [HttpPatch("handleLike/mealId/{id}/count/{count}")]
        [Authorize]
        public async Task<IActionResult> PatchHandleLike(int id, int count)
        {
            var meal = AllMeal.FirstOrDefault(x => x.Id == id);
            if (meal == null)
                return BadRequest();

            meal.Like += count;
            return (await _db.SaveChangesAsync() > 0) ? Ok("Success") : BadRequest();
        }

        [HttpPatch("handleDislike/mealId/{id}/count/{count}")]
        [Authorize]
        public async Task<IActionResult> PatchHandleDislike(int id, int count)
        {
            var meal = AllMeal.FirstOrDefault(x => x.Id == id);
            if (meal == null)
                return BadRequest();

            meal.Dislike += count;
            return (await _db.SaveChangesAsync() > 0) ? Ok("Success") : BadRequest();
        }
        #endregion

        #region DELETE
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin, Support, Customer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest("Recept Id saknas ... ");

            var Meal = _db.Meal.FirstOrDefault(x => x.Id == id);
            if (Meal == null)
                return BadRequest("Recept med matchande Id kunde inte hittats");
            try
            {
                var img = Meal.ImgUrl;
                _db.Meal.Remove(Meal);
                await _db.SaveChangesAsync();
                if (!_help.DeleteImage(img))
                    return Ok("Recept är bortagen men bilden kunde inte raderas!");

                return Ok("Recept har tagits bort ...");
            }
            catch (Exception e)
            {
                return BadRequest("Något har gått snett, var vänlig försök senare ...  <br/> Errorn => " + e.Message);
            }
        }
        #endregion

        #region Helpers
        // Return week number
        public int ReturnWeekNumber(DateTime date)
        {

            return _cal.GetWeekOfYear(date, _cwr, _dow);
        }

        // Return weeks count of year
        public int ReturnWeeksCount(DateTime date)
        {
            DateTime LastDay = new System.DateTime(date.Year, 12, 31);
            return _cal.GetWeekOfYear(LastDay, _cwr, _dow);
        }

        // Return day of week
        public string ReturnDayOfWeek(DateTime date)
        {
            var day = _ci.DateTimeFormat.GetDayName(date.DayOfWeek);
            var firstLetter = day.Substring(0, 1);
            return firstLetter.ToUpper() + day.Substring(1);
        }
        #endregion
    }
}
