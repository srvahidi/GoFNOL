using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GoFNOL.Models;
using GoFNOL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GoFNOL.Controllers
{
	[Route("api/[controller]")]
	public class FNOLController : Controller
	{
		[HttpPost]
		public async Task<IActionResult> Post([FromBody] FNOLRequest request, [FromServices] IFNOLService fnolService, [FromServices] ILogger<FNOLController> logger)
		{
			try
			{
				var response = await fnolService.CreateAssignment(request);
				return Json(response);
			}
			catch (EAIException x)
			{
				logger.LogError(x, "Returning BadGateway.");
				return StatusCode((int) HttpStatusCode.BadGateway);
			}
			catch (HttpRequestException x)
			{
				logger.LogError(x, "Returning GatewayTimeout.");
				return StatusCode((int) HttpStatusCode.GatewayTimeout);
			}
			catch (Exception x)
			{
				logger.LogError(x, "Returning InternalServerError.");
				return StatusCode((int) HttpStatusCode.InternalServerError);
			}
		}
	}
}