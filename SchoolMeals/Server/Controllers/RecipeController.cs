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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeRecipe.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _host;
        private readonly IHelpers _help;

        public RecipeController(
                        AppDbContext db,
                        IHelpers help,
                        IWebHostEnvironment host
            )
        {
            _db = db;
            _help = help;
            _host = host;
        }

        private IEnumerable<Recipe> AllRecipe
        {
            get
            {
                return _db.Recipe.Include(s => s.School).ToList();
            }
        }


        #region GET
        // GET: All Recipe by school id for support
        [HttpGet("GetBySchoolId/{id}")]
        //[Authorize(Roles = "Admin, Support")]
        public IEnumerable<Recipe> GetRecipeBySchool(int id)
        {
            return AllRecipe.Where(x => x.School.Id == id).ToList();
        }

        // GET: GET Recipe by id (support or customer access)
        [HttpGet("GetBySchoolIdAndRecipeId/{schoolId}/{recipeId}")]
        //[Authorize(Roles = "Admin, Support")]
        public Recipe GetRecipeBySchoolAndId(int schoolId, int recipeId)
        {
            return AllRecipe.FirstOrDefault(x => x.Id == recipeId && x.School.Open && x.School.Id == schoolId);
        }
        #endregion

        #region POST
        // POST New Recipe
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
                        imgName = _help.ImgName("recipies/" + model.Name, model.FileName);
                        // Save image
                        upload = _help.SaveFile(model.File, imgName);
                    }

                    var school = _db.School.FirstOrDefault(x => x.Id == model.SchoolId);
                    if (school == null)
                        return BadRequest("Skola med matchande Id kunde inte hittats ...");

                    var Recipe = new Recipe
                    {
                        Name = model.Name,
                        Text = model.Text,
                        Ingredients = model.Ingredients,
                        School = school,
                        ImgUrl = (upload is bool && upload) ? imgName : ""
                    };
                    _db.Recipe.Add(Recipe);
                    await _db.SaveChangesAsync();

                    if (upload is string)
                        return Ok("Data skickades framgångsrikt men bilden kunde inte sparas! <br/>Error => " + upload);

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
                        imgName = _help.ImgName("recipies/" + model.Name, model.FileName);
                        // Save image
                        upload = _help.SaveFile(model.File, imgName);
                    }

                    var recipe = AllRecipe.FirstOrDefault(x => x.Id == id);
                    if (recipe == null)
                        return BadRequest("Recept med matchande Id kunde inte hittats");

                    recipe.Name = model.Name;
                    recipe.Ingredients = model.Ingredients;
                    recipe.Text = model.Text;
                    if(upload is bool && upload)
                    {
                        imgToRemove = recipe.ImgUrl;
                        recipe.ImgUrl = imgName;
                    }

                    await _db.SaveChangesAsync();
                    if (upload is bool && upload)
                        _help.DeleteImage(imgToRemove);


                    if (upload != null && upload is string)
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
        #endregion

        #region DELETE
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin, Support, Customer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest("Recept Id saknas ... ");

            var Recipe = _db.Recipe.FirstOrDefault(x => x.Id == id);
            if (Recipe == null)
                return BadRequest("Recept med matchande Id kunde inte hittats");
            var Meals = _db.Meal.Where(x => x.RecipeId == Recipe.Id).ToList();
            try
            {
                var img = Recipe.ImgUrl;
                _db.Recipe.Remove(Recipe);
                if(Meals.Count > 0)
                {
                    foreach (var m in Meals)
                        m.RecipeId = 0;
                }
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
        #endregion
    }
}
