using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GoFNOL.Models;
using GoFNOL.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoFNOL.Controllers
{
	[Route("api/[controller]")]
	public class FNOLController : Controller
	{
		[HttpPost]
		public async Task<IActionResult> Post([FromBody] FNOLRequest request, [FromServices] IFNOLService fnolService)
		{
			try
			{
				var response = await fnolService.CreateAssignment(request);
				return Json(response);
			}
			catch (EAIException)
			{
				return StatusCode((int) HttpStatusCode.BadGateway);
			}
			catch (HttpRequestException)
			{
				return StatusCode((int) HttpStatusCode.GatewayTimeout);
			}
			catch (Exception)
			{
				return StatusCode((int) HttpStatusCode.InternalServerError);
			}
		}
	}
}