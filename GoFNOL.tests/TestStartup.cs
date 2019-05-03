using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace GoFNOL.tests
{
	public class TestStartup : Startup
	{
		public TestStartup(IConfiguration configuration) : base(configuration)
		{
		}

		public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			app.UseMiddleware<TestAuthMiddleware>();
			Configure(app, env, false);
		}
	}
}