using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GoFNOL.Models;
using GoFNOL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GoFNOL.Controllers
{
	[Authorize]
	public class FNOLController : Controller
	{
		private readonly IFNOLService fnolService;

		private readonly ILogger<FNOLController> logger;

		public FNOLController(IFNOLService fnolService, ILogger<FNOLController> logger)
		{
			this.fnolService = fnolService;
			this.logger = logger;
		}

		public IActionResult Index()
		{
			var host = Request.Host.Host;
			var environmentDisplay = GetEnvironmentDisplay(host, "Local");
			ViewData["environmentDisplay"] = environmentDisplay;

			return View();
		}

		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		[HttpPost("fnol")]
		public async Task<IActionResult> Create([FromForm] IFormCollection formBody)
		{
			try
			{
				var request = new FNOLRequest
				{
					ClaimNumber = formBody["claim-number"],
					MobileFlowIndicator = formBody["mobile-flow-ind"],
					VIN = formBody["vin"],
					LossType = formBody["loss-type"],
					Deductible = formBody["deductible-waived"] == "on" ? "W" : (string)formBody["deductible"],
					Owner = new Party
					{
						FirstName = formBody["first-name"],
						LastName = formBody["last-name"],
						PhoneNumber = formBody["phone-number"],
						Email = formBody["email"],
						Address = new Address
						{
							PostalCode = $"{(string)formBody["zip-code"]}-{(string)formBody["zip-addon"]}",
							State = formBody["state"]
						}
					}
				};
				var sw = Stopwatch.StartNew();
				var claim = await fnolService.CreateAssignment(request);
				var workAssignmentId = claim.WorkAssignmentId;
				TempData["result"] = $"Work Assignment ID: '{workAssignmentId}' added successfully!";
				TempData["claim"] = $"Claim Number: '{request.ClaimNumber}'.";
				TempData["responseTime"] = $"It took {(int)sw.Elapsed.TotalSeconds} seconds";
			}
			catch (Exception x)
			{
				logger.LogError(x, "Error during FNOL process");
			}
			return Redirect("/");
		}

		private static string GetEnvironmentDisplay(string host, string defaultDisplay)
		{
			if (string.IsNullOrWhiteSpace(host))
			{
				return defaultDisplay;
			}

			host = host.ToLower();

			if (host.Contains("-demo")) // NOTE: check for this first because demo host may contain string `prod`
			{
				return "Demo";
			}

			if (host.Contains("-acceptance"))
			{
				return "Acceptance";
			}

			if (host.Contains("-int"))
			{
				return "Int";
			}

			return defaultDisplay;
		}
	}
}
