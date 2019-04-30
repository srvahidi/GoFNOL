using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GoFNOL.tests
{
	public class TestAuthMiddleware
	{
		private readonly RequestDelegate next;

		public TestAuthMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var identity = new ClaimsIdentity(new List<Claim>(), "TestAuthType");
			var claimsPrincipal = new ClaimsPrincipal(identity);
			context.User = claimsPrincipal;
			await next(context);
		}
	}
}