using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace GoFNOL.tests
{
	public class TestStartup : Startup
	{
		public TestStartup(IConfiguration configuration, IHostingEnvironment hostingEnvironment) : base(configuration, hostingEnvironment)
		{
		}

		public override void Configure(IApplicationBuilder app)
		{
			app.UseMiddleware<TestAuthMiddleware>();
			base.Configure(app);
		}
	}
}