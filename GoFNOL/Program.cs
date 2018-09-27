using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace GoFNOL
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((context, builder) =>
				{
					builder.AddEnvironmentVariables();
					if (context.HostingEnvironment.IsDevelopment())
					{
						builder.AddUserSecrets("GoFNOL");
					}
				})
				.UseStartup<Startup>()
				.Build();
	}
}