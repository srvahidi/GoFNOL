using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace GoFNOL.tests
{
	public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
	{
		protected override IWebHostBuilder CreateWebHostBuilder()
		{
			return new WebHostBuilder()
				.ConfigureAppConfiguration((context, builder) =>
				{
					builder.AddEnvironmentVariables();
					if (context.HostingEnvironment.IsDevelopment())
					{
						builder.AddUserSecrets("GoFNOL");
					}
				})
				.UseStartup<TestStartup>();
		}
	}
}