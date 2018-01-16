using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using GoFNOL.Models;
using GoFNOL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoFNOL.Controllers
{
    public class FNOLController : Controller
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
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        [HttpPost("fnol")]
        public async Task<IActionResult> Create([FromForm] IFormCollection formBody)
        {
            var request = new FNOLRequest
            {
                ClaimNumber = formBody["claim-number"],
                VIN = formBody["vin"],
                LossType = formBody["loss-type"],
                Deductible = formBody["deductible"],
                Owner = new Party
                {
                    FirstName = formBody["first-name"],
                    LastName = formBody["last-name"],
                    PhoneNumber = formBody["phone-number"],
                    Email = formBody["email"],
                    Address = new Address
                    {
                        PostalCode = $"{(string) formBody["zip-code"]}-{(string) formBody["zip-addon"]}",
                        State = formBody["state"]
                    }
                }
            };
            var fnol = new FNOLTool();
            var sw = Stopwatch.StartNew();
            var claim = await fnol.CreateAssignment(request);
            var workAssignmentId = claim.WorkAssignmentId;
            TempData["result"] = $"Work Assignment ID: '{workAssignmentId}' added successfully!";
            TempData["claim"] = $"Claim: '{request.ClaimNumber}'.";
            TempData["responseTime"] = $"It took {(int) sw.Elapsed.TotalSeconds} seconds";
            return Redirect(magicGuid);
        }
    }
}