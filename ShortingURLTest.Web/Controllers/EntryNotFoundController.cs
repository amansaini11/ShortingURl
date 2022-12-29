using Microsoft.AspNetCore.Mvc;

namespace ShortingURLTest.Web.Controllers
{
    public class EntryNotFoundController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
