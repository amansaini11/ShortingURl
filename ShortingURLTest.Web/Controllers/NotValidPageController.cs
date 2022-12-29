using Microsoft.AspNetCore.Mvc;

namespace ShortingURLTest.Web.Controllers
{
    public class NotValidPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
