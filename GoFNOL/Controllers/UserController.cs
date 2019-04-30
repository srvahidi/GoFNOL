using System.Threading.Tasks;
using GoFNOL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace GoFNOL.Controllers
{
	[Route("api/[controller]")]
	public class UserController : Controller
	{
		[HttpGet("{userName}")]
		public async Task<IActionResult> GetData([FromRoute] string userName, [FromServices] INGPService ngpService, [FromServices] ILogger<UserController> logger)
		{
			var userProfileId = await ngpService.GetUserProfileIdAsync(userName);
			logger.LogInformation($"Profile id = {userProfileId} returned for user id = {userName}");
			var userData = new JObject
			{
				["profileId"] = userProfileId
			};
			return Json(userData);
		}
	}
}