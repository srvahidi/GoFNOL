using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using web_fnol_tool.Models;
using FnolTools;
using Microsoft.AspNetCore.Http;

namespace web_fnol_tool.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] IFormCollection formBody)
        {
            var fnol = new FnolTool();
            await fnol.CreateAssignment((string) formBody["claim-number"]);

            TempData["Success"] = "Added Successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
