using System.Diagnostics;
using System.Threading.Tasks;
using GoFNOL.Models;
using GoFNOL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoFNOL.Controllers
{
    public class FNOLController : Controller
    {
        private readonly IHTTPService httpService;

        public FNOLController(IHTTPService httpService)
        {
            this.httpService = httpService;
        }

        public IActionResult Index()
        {
            return View();
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
                MobileFlowIndicator = formBody["mobile-flow-ind"],
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
            var fnol = new FNOLTool(httpService);
            var sw = Stopwatch.StartNew();
            var claim = await fnol.CreateAssignment(request);
            var workAssignmentId = claim.WorkAssignmentId;
            TempData["result"] = $"Work Assignment ID: '{workAssignmentId}' added successfully!";
            TempData["claim"] = $"Claim: '{request.ClaimNumber}'.";
            TempData["responseTime"] = $"It took {(int) sw.Elapsed.TotalSeconds} seconds";
            return Redirect("/");
        }
    }
}