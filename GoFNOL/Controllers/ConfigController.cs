using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace GoFNOL.Controllers
{
	[Route("api/[controller]")]
	public class ConfigController : Controller
	{
		[HttpGet, AllowAnonymous]
		public IActionResult Get([FromServices] IEnvironmentConfiguration envConfig)
		{
			var jConfig = new JObject
			{
				["isEndpoint"] = envConfig.ISEndpoint,
				["disableAuth"] = envConfig.DisableAuth
			};

			return Json(jConfig);
		}
	}
}