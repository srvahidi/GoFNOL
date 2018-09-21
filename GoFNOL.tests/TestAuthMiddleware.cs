using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GoFNOL.tests
{
	public class TestAuthMiddleware
	{
		public const string Username = "pe2generic1";

		private readonly RequestDelegate next;

		public TestAuthMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var claims = new List<Claim>
			{
				new Claim("sub", Username)
			};
			var identity = new ClaimsIdentity(claims, "TestAuthType");
			var claimsPrincipal = new ClaimsPrincipal(identity);
			context.User = claimsPrincipal;
			await next(context);
		}
	}
}