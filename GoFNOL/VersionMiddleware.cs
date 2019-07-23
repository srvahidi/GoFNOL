using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GoFNOL
{
	public class VersionMiddleware
	{
		private readonly RequestDelegate _next;

		private readonly ILogger<VersionMiddleware> _logger;

		public VersionMiddleware(RequestDelegate next, ILogger<VersionMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (context.Request.Path.StartsWithSegments("/api/version"))
			{
				try
				{
					// following file is provided by ci/cd scripts
					var version = await File.ReadAllTextAsync("version.txt");
					await context.Response.WriteAsync(version);
				}
				catch (Exception x)
				{
					_logger.LogError(x, "Failed to read version from file");
				}
			}
			else
			{
				await _next(context);
			}
		}
	}
}