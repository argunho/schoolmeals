using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;


namespace SchoolMeals.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SchoolController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _host;
        private readonly IHelpers _help;
        private readonly UserManager<Users> _userManager;

        public SchoolController(
                        AppDbContext db,
                        IHelpers help,
                        IWebHostEnvironment host,
            UserManager<Users> userManager
            )
        {
            _db = db;
            _help = help;
            _userManager = userManager;
            _host = host;
        }

        private IEnumerable<School> AllSchool
        {
            get
            {
                return _db.School.Include(p => p.Municipality).Include(b => b.Bookmarks).OrderByDescending(x => x.Id).ToList();
            }
        }

        #region GET
        // GET: All School by Open parameter
        [HttpGet]
        //[Authorize(Roles = "Admin, Support")]
        public IEnumerable<School> Get()
        {
            return AllSchool.Where(x => x.Open).ToList();
        }

        // GET: All school
        [HttpGet("schools")]
        [Authorize(Roles = "Admin, Support")]
        public IEnumerable<School> AllSchoolsList()
        {
            return AllSchool;
        }

        // GET: School by id
        [HttpGet("{id}")]
        //[Authorize(Roles = "Admin, Support, Customer")]
        public School Get(int id)
        {
            return _db.School.Include(m => m.Municipality).Include(u => u.Users).Include(meal => meal.Meals)
                        .Include(r => r.Recipies).FirstOrDefault(x => x.Id == id);
        }

        // GET: Get school by schol name, place and municipality
        [HttpGet("GetByParameters/{municipality}/{place}/{name}")]
        public School GetSchoolByParameters(string municipality, string place, string name)
        {
            var link = municipality + "/" + place + "/" + name;
            return _db.School.Include(m => m.Municipality).FirstOrDefault(x => x.Link.ToLower() == link.ToLower() && x.Open);
        }

        // GET: Get all municipality
        [HttpGet("GetMunicipalities")]
        public IEnumerable<Municipality> GetMunicipalities()
        {
            var municipalities = _db.Municipality.OrderBy(x => x.Name).ToList();
            if (municipalities.Count == 0)
            {
                string arr = ("Ale,Alingsås,Alvesta,Aneby,Arboga,Arjeplogs,Arvidsjaurs,Arvika,Askersunds,Avesta,Bengtsfors,Bergs,Bjurholms,Bjuvs,Bodens,Bollebygds,Bollnäs,Borgholms,Borlänge,Borås stad,Botkyrka,Boxholms,Bromölla,Bräcke,Burlövs,Båstads,Dals-Eds,Danderyds,Degerfors,Dorotea,Eda,Ekerö,Eksjö,Emmaboda,Enköpings,Eskilstuna,Eslövs,Essunga,Fagersta,Falkenbergs,Falköpings,Falu,Filipstads,Finspångs,Flens,Forshaga,Färgelanda,Gagnefs,Gislaveds,Gnesta,Gnosjö,Region Gotland,Grums,Grästorps,Gullspångs,Gällivare,Gävle,Göteborgs stad,Götene,Habo,Hagfors,Hallsbergs,Hallstahammars,Halmstads,Hammarö,Haninge,Haparanda stad,Heby,Hedemora,Helsingborgs stad,Herrljunga,Hjo,Hofors,Huddinge,Hudiksvalls,Hultsfreds,Hylte,Håbo,Hällefors,Härjedalens,Härnösands,Härryda,Hässleholms,Höganäs,Högsby,Hörby,Höörs,Jokkmokks,Järfälla,Jönköpings,Kalix,Kalmar,Karlsborgs,Karlshamns,Karlskoga,Karlskrona,Karlstads,Katrineholms,Kils,Kinda,Kiruna,Klippans,Knivsta,Kramfors,Kristianstads,Kristinehamns,Krokoms,Kumla,Kungsbacka,Kungsörs,Kungälvs,Kävlinge,Köpings,Laholms,Landskrona stad,Laxå,Lekebergs,Leksands,Lerums,Lessebo,Lidingö stad,Lidköpings,Lilla Edets,Lindesbergs,Linköpings,Ljungby,Ljusdals,Ljusnarsbergs,Lomma,Ludvika,Luleå,Lunds,Lycksele,Lysekils,Malmö stad,Malung-Sälens,Malå,Mariestads,Markaryds,Marks,Melleruds,Mjölby,Mora,Motala,Mullsjö,Munkedals,Munkfors,Mölndals stad,Mönsterås,Mörbylånga,Nacka,Nora,Norbergs,Nordanstigs,Nordmalings,Norrköpings,Norrtälje,Norsjö,Nybro,Nykvarns,Nyköpings,Nynäshamns,Nässjö,Ockelbo,Olofströms,Orsa,Orust,Osby,Oskarshamns,Ovanåkers,Oxelösunds,Pajala,Partille,Perstorps,Piteå,Ragunda,Robertsfors,Ronneby,Rättviks,Sala,Salems,Sandvikens,Sigtuna,Simrishamns,Sjöbo,Skara,Skellefteå,Skinnskattebergs,Skurups,Skövde,Smedjebackens,Sollefteå,Sollentuna,Solna stad,Sorsele,Sotenäs,Staffanstorps,Stenungsunds,Stockholms stad,Storfors,Storumans,Strängnäs,Strömstads,Strömsunds,Sundbybergs stad,Sundsvalls,Sunne,Surahammars,Svalövs,Svedala,Svenljunga,Säffle,Säters,Sävsjö,Söderhamns,Söderköpings,Södertälje,Sölvesborgs,Tanums,Tibro,Tidaholms,Tierps,Timrå,Tingsryds,Tjörns,Tomelilla,Torsby,Torsås,Tranemo,Tranås,Trelleborgs,Trollhättans stad,Trosa,Tyresö,Täby,Töreboda,Uddevalla,Ulricehamns,Umeå,Upplands Väsby,Upplands-Bro,Uppsala,Uppvidinge,Vadstena,Vaggeryds,Valdemarsviks,Vallentuna,Vansbro,Vara,Varbergs,Vaxholms stad,Vellinge,Vetlanda,Vilhelmina,Vimmerby,Vindelns,Vingåkers,Vårgårda,Vänersborgs,Vännäs,Värmdö,Värnamo,Västerviks,Västerås stad,Växjö,Ydre,Ystads,Åmåls,Ånge,Åre,Årjängs,Åsele,Åstorps,Åtvidabergs,Älmhults,Älvdalens,Älvkarleby,Älvsbyns,Ängelholms,Öckerö,Ödeshögs,Örebro,Örkelljunga,Örnsköldsviks,Östersunds,Österåkers,Östhammars,Östra Göinge,Överkalix,Övertorneå");

                foreach (var r in arr.Split(",").ToList())
                {
                    _db.Municipality.Add(new Municipality { Name = r });
                    _db.SaveChanges();
                }

                municipalities = _db.Municipality.OrderBy(x => x.Name).ToList();
            }

            return municipalities;
        }

        // GET: Get municipality name by school id
        [HttpGet("GetMunicipalityBySchoolId/{id}")]
        public Municipality GetMuncipaltyName(int id)
        {
            var school = AllSchool.FirstOrDefault(x => x.Id == id);
            if (school.Municipality != null)
                return school.Municipality;

            return null;
        }

        // GET: Multiple school list by search keyword
        [HttpGet("GetSchoolAndMunicipality/{name}")]
        public List<School> GetSchoolAndMuncipalityBySearch(string name)
        {
            List<School> result = new List<School>();
            var schoolResult = _db.School.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
            var municipalityResult = _db.School.Include(x => x.Municipality).Where(x => x.Municipality.Name.ToLower().Contains(name.ToLower())).ToList();
            foreach (var s in schoolResult)
                result.Add(s);
            foreach (var ms in municipalityResult)
                result.Add(ms);

            return result;
        }

        // GET: All school
        [HttpGet("bookmarks/{user}")]
        [Authorize]
        public IEnumerable<School> GetBookmarkedSchools(string user)
        {
            return _db.School.Include(p => p.Municipality).Include(b => b.Bookmarks)
                    .Where(x => x.Bookmarks.Any(b => b.UserId == user)).ToList();
        }
        #endregion

        #region POST
        // POST New School
        [HttpPost]
        //[Authorize(Roles = "Admin, Support")]
        public async Task<IActionResult> Post(SchoolViewModel model)
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
                        imgName = _help.ImgName("schools/" + model.Name, model.FileName);
                        // Save image
                        upload = _help.SaveFile(model.File, imgName);
                    }

                    var municipality = _db.Municipality.FirstOrDefault(x => x.Id == model.MunicipalityId);
                    var user = _db.Users.Include(s => s.School).FirstOrDefault(x => x.Email == model.UserEmail);
                    var school = new School
                    {
                        Name = model.Name,
                        Text = model.Text,
                        Place = model.Place,
                        Address = model.Address,
                        Zip = model.Zip,
                        Municipality = municipality,
                        Link = ReturnLink(municipality.Name, model.Place, model.Name),
                        Open = model.Open,
                        ImgUrl = (upload is bool && upload) ? imgName : ""
                    };
                    _db.School.Add(school);
                    await _db.SaveChangesAsync();

                    user.School = school;
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
        public async Task<IActionResult> Put(int id, SchoolViewModel model)
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
                        imgName = _help.ImgName("schools/" + model.Name, model.FileName);
                        // Save image
                        upload = _help.SaveFile(model.File, imgName);
                    }


                    var school = AllSchool.FirstOrDefault(x => x.Id == id);
                    if (school == null)
                        return BadRequest("Skolan med matchande Id kunde inte hittats");

                    school.Name = model.Name;
                    school.Text = model.Text;
                    school.Place = model.Place;
                    school.Address = model.Address;
                    school.Zip = model.Zip;
                    if (school.Municipality.Id != model.MunicipalityId)
                        school.Municipality = _db.Municipality.FirstOrDefault(x => x.Id == model.MunicipalityId);
                    school.Link = ReturnLink(school.Municipality.Name, model.Place, model.Name);
                    school.Open = model.Open;
                    if (upload is bool && upload)
                    {
                        imgToRemove = school.ImgUrl;
                        school.ImgUrl = imgName;
                    }

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
        #endregion

        #region PATCH
        // Patch: Make school like a bookmark
        [HttpPatch("makeBookmark/{id}/{userId}")]
        [Authorize]
        public async Task<IActionResult> MakeBookmark(int id, string userId)
        {
            var school = _db.School.Include(b => b.Bookmarks).FirstOrDefault(x => x.Id == id);
            var user = _db.Users.FirstOrDefault(x => x.Id == userId);
            if (school != null && user != null)
            {
                var bookmark = new Bookmarks
                {
                    School = school,
                    UserId = user.Id
                };
                _db.Bookmarks.Add(bookmark);
                var res = await _db.SaveChangesAsync();
                if (res > 0)
                    return Ok("Success");
                else
                    return BadRequest("Det gick något fel ....");
            }
            return BadRequest("Kunde inte hittats school eller användare med matchande id ...");
        }
        #endregion

        #region DELETE
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin, Support, Customer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest("Skolans Id saknas ... ");

            var school = _db.School.Include(u => u.Users).FirstOrDefault(x => x.Id == id);
            if (school == null)
                return BadRequest("Skolan med matchande Id kunde inte hittats");
            try
            {
                var img = school.ImgUrl;
                if (school.Users.Count() > 0)
                {
                    foreach (var u in school.Users)
                        u.School = null;
                    await _db.SaveChangesAsync();
                }
                _db.School.Remove(school);
                await _db.SaveChangesAsync();
                if (!_help.DeleteImage(img))
                    return Ok("Skolan är bortagen men bilden kunde inte raderas!");

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest("Något har gått snett, var vänlig försök senare ...  <br/> Errorn => " + e.Message);
            }
        }

        [HttpDelete("deleteBookmark/{id}/{userId}")]
        [Authorize]
        public async Task<IActionResult> DeleteBookmark(int? id, string userId)
        {
            if (id == null)
                return BadRequest("Bookmark Id saknas ... ");

            var bookmark = _db.Bookmarks.Include(s => s.School).FirstOrDefault(x => x.School.Id == id && x.UserId == userId);
            if (bookmark == null)
                return BadRequest("Skolan med matchande Id kunde inte hittats");
            try
            {
                _db.Bookmarks.Remove(bookmark);
                await _db.SaveChangesAsync();
                return Ok("Success");
            }
            catch (Exception e)
            {
                return BadRequest("Något har gått snett, var vänlig försök senare ...  <br/> Errorn => " + e.Message);
            }
        }
        #endregion

        #region Helpers
        // Check school link
        private bool CheckSchoolLink(string link)
        {
            return AllSchool.FirstOrDefault(x => x.Link == link) != null;
        }

        // Return corrected link
        private string ReturnLink(string muniplaicty, string place, string name)
        {
            var link = (muniplaicty + "/" + place + "/" + name).Replace(" ", "_").Replace("ä", "a").Replace("ö", "o").Replace("å", "a");

            return link.ToLower();
        }
        #endregion
    }
}
