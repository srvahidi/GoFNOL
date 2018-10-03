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
			var response = await fnolService.CreateAssignment(request);
			return Json(response);
		}
	}
}