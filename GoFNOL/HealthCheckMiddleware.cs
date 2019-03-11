using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GoFNOL
{
	public class HealthCheckMiddleware
	{
		private readonly RequestDelegate _next;

		public HealthCheckMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (!context.Request.Path.StartsWithSegments("/api/health"))
				await _next(context);
		}
	}
}