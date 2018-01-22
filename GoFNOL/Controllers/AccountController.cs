using Microsoft.AspNetCore.Mvc;

namespace GoFNOL.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
