using System.Linq;
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
		[HttpGet("data")]
		public async Task<IActionResult> GetData([FromServices] INGPService ngpService, [FromServices] ILogger<UserController> logger)
		{
			var subClaim = User.Claims.FirstOrDefault(c => c.Type == "sub");
			if (subClaim == null)
			{
				logger.LogError("Sub claim was not found for current user");
			}

			var userProfileId = await ngpService.GetUserProfileIdAsync(subClaim.Value);
			var userData = new JObject
			{
				["profileId"] = userProfileId
			};
			return Json(userData);
		}

		[HttpPost("logout")]
		public IActionResult PostLogout()
		{
			return SignOut("Cookies", "oidc");
		}
	}
}