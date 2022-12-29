
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShortingURL.Web.Models;
using ShortingURLTest.Web.Context;
using ShortingURLTest.Web.Entity;
using ShortingURLTest.Web.Models;
using System.Diagnostics;


namespace ShortingURLTest.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        private readonly string _baseURL = "https://localhost:7275/";

        public HomeController(ILogger<HomeController> logger,
            RoleManager<AppRole> roleManager,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            DataContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;

            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            List<URLMapping> query = new List<URLMapping>();
            var result = _signInManager.IsSignedIn(User);
            if (result == true)
            {
                var users = _context.AppUsers.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                query = _context.UrlMappings.Where(x => x.AppUserId == users.Id).ToList();
                ViewBag.user = users;
                return View(query);
            }
            TempData["orignal"] = Request.Cookies["Orignalurl"];
            TempData["lilliurl"] = Request.Cookies["LilliUrl"];
            query = _context.UrlMappings.ToList();

            return View(query);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult EntryNotFound()
        {
            return View();
        }
        public IActionResult NotValidPage()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> SaveURLMapping(string url, string alias, int userId)
        {

            if (!string.IsNullOrEmpty(url))
            {
                URLMapping urlMapping = new URLMapping();
                if (!string.IsNullOrEmpty(alias))
                {
                    bool checkUrl = CheckUrlExists(alias);
                    if (checkUrl != true)
                        urlMapping.TinyUrl = alias;
                    else
                        return Json(false);
                }
                else
                {
                    urlMapping.TinyUrl = GenerateTinyURL();
                    bool checkUrl = CheckUrlExists(urlMapping.TinyUrl);
                    if (checkUrl == true)
                        await SaveURLMapping(url, alias, userId);

                }
                //addded cookies
                CookieOptions obj = new CookieOptions();
                obj.Expires = DateTime.Now.AddYears(10);
                Response.Cookies.Append("Orignalurl", url, obj);
                Response.Cookies.Append("LilliUrl", urlMapping.TinyUrl, obj);

                if (userId == 0)
                {
                    urlMapping.Orignalurl = url;
                    urlMapping.AppUserId = null;
                }
                else
                {
                    urlMapping.Orignalurl = url;
                    urlMapping.AppUserId = userId;
                }
                _context.UrlMappings.Add(urlMapping);
                var result = await _context.SaveChangesAsync();
                return Json(_baseURL + urlMapping.TinyUrl);
            }
            else
            {
                return Json(false);
            }

        }
        [HttpPost]
        public async Task<JsonResult> Register(string name, string email, string password)
        {
            if (!string.IsNullOrEmpty(email))
            {
                bool checkEmail = CheckEmailExists(email);
                if (checkEmail != true)
                {
                    //await Register(name,email,password);
                }
                else
                    return Json(false);
            }

            var user = new AppUser();
            user.FirstName = name;
            user.Email = email;
            user.UserName = name.Trim().Replace(" ", "").ToLower();
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                string errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += error.Description + Environment.NewLine;
                }
                return Json("Your password must be 6 characters and contain at least one symbol(!, @, #, etc)");
            }
            return Json(true);
        }
        [HttpPost]
        public async Task<ActionResult> Login(string email, string password)
        {
            var user = await _userManager.Users
            .SingleOrDefaultAsync(x => x.Email == email.ToLower());
            if (user == null)
            {
                return Json("Wrong Email-Id");
            }
            if (user.IsDeleted == true)
            {
                return Json("Account Deleted");
            }
            var result = await _signInManager
               .CheckPasswordSignInAsync(user, password, false);

            if (!result.Succeeded)
            {
                return Json("Email & Password dosn't match");
            }
            await _signInManager.SignInAsync(user, isPersistent: false);
            return Json(true);

        }
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("");
        }
        [HttpGet]
        public IActionResult Profile()
        {
            var result = _signInManager.IsSignedIn(User);
            if (result == true)
            {
                var users = _context.AppUsers.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                var query = _context.UrlMappings.Where(x => x.AppUserId == users.Id).ToList();
                ViewBag.user = users;
                return PartialView("_ProfilePartial", query);
            }
            return RedirectToAction("");
        }
        public IActionResult RemoveUser(int id)
        {
            var user = _userManager.Users.Where(_x => _x.Id == id).FirstOrDefault();
            if (user != null)
            {
                user.IsDeleted = true;
                _context.SaveChanges();
                return Json("Account deleted Success");
            }
            return BadRequest();

        }

        [HttpGet]
        public IActionResult MyUrls()
        {
            var result = _signInManager.IsSignedIn(User);
            if (result == true)
            {
                var user = _context.AppUsers.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                ViewBag.url = _context.UrlMappings.Where(x => x.AppUserId == user.Id).ToList();
                return PartialView("_MyUrlsPartial", new { ViewBag.user });
            }
            return PartialView("_MyUrlsPartial");
        }

        [HttpPost]
        public IActionResult MyUrls(AppUser appUser)
        {
            var result = _signInManager.IsSignedIn(User);
            if (result == true)
            {
                var user = _context.AppUsers.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                ViewBag.url = _context.UrlMappings.Where(x => x.AppUserId == user.Id).ToList();
                return PartialView("_MyUrlsPartial", new { ViewBag.user });
            }
            return PartialView("_MyUrlsPartial");
        }
        public IActionResult RemoveURL(int id)
        {
            var url = _context.UrlMappings.Where(x => x.Id == id).FirstOrDefault();
            _context.Remove(url);
            _context.SaveChanges();
            return Json("Remove Success");
        }

        [HttpPost]
        public async Task<JsonResult> ForgetPassword(string email, string password)
        {
            if (!string.IsNullOrEmpty(email))
            {
                bool checkEmail = CheckEmailExists(email);
                if (checkEmail == true)
                {
                    var user = await _userManager.FindByNameAsync(email);
                    var Record = _context.AppUsers.Where(x => x.Email == email).FirstOrDefault();
                    if (Record != null)
                    {
                        Record.PasswordHash = _userManager.PasswordHasher.HashPassword(user, password); 
                        _context.AppUsers.Update(Record);
                        await _context.SaveChangesAsync();
                        return Json("Password Update Success");
                    }
                }
                else
                    return Json(false);
            }
            return Json(false);
        }
        private Task<bool> UserExists(object email)
        {
            throw new NotImplementedException();
        }

        private string GenerateTinyURL(int length = 6)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public bool CheckUrlExists(string url)
        {
            var checkUrl = _context.UrlMappings.Where(x => x.TinyUrl == url).FirstOrDefault();
            if (checkUrl != null)
            {
                return true;
            }
            return false;
        }
        private bool CheckEmailExists(string email)
        {
            var checkEmail = _context.AppUsers.Where(x => x.Email == email).FirstOrDefault();
            if (checkEmail != null)
            {
                return true;
            }
            return false;
        }
    }
}
