using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GoFNOL
{
	public class UserSpoofMiddleware
	{
		private readonly RequestDelegate next;

		public UserSpoofMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var claims = new List<Claim>
			{
				new Claim("sub", "pe2generic1")
			};
			var identity = new ClaimsIdentity(claims, "TestAuthType");
			context.User = new ClaimsPrincipal(identity);
			await next(context);
		}
	}
}