using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using web_fnol_tool.Models;
using FnolTools;
using Microsoft.AspNetCore.Http;

namespace web_fnol_tool.Controllers
{
    public class HomeController : Controller
    {
        private readonly string magicGuid = Encoding.ASCII.GetString(Convert.FromBase64String("NjdlNGQyZWMwZTZjNGRjZTgxYWNkYWYyMGM0NzQ2NTc="));

        [Route("{guid}")]
        public IActionResult Index([FromRoute] string guid)
        {
            if (guid == magicGuid)
            {
                return View();
            }

            return NotFound();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] IFormCollection formBody)
        {
            var fnol = new FnolTool();
            var claimNum = (string) formBody["claim-number"];
            var sw = Stopwatch.StartNew();
            await fnol.CreateAssignment(claimNum);
            var workAssignmentId = fnol.Claim.WorkAssignmentId;
            TempData["result"] = $"Work Assignment ID: '{workAssignmentId}' added successfully!";
            TempData["claim"] = $"Claim: '{claimNum}'.";
            TempData["responseTime"] = $"It took {(int) sw.Elapsed.TotalSeconds} seconds";
            return Redirect(magicGuid);
        }
    }
}
