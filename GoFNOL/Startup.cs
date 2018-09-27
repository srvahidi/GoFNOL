using System.IdentityModel.Tokens.Jwt;
using GoFNOL.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GoFNOL
{
	public class Startup
	{
		private readonly IHostingEnvironment hostingEnvironment;

		private readonly IConfiguration configuration;

		public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
		{
			this.hostingEnvironment = hostingEnvironment;
			this.configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			if (hostingEnvironment.IsDevelopment() || hostingEnvironment.IsStaging())
			{
				services.AddMvc(options => options.Filters.Add<AllowAnonymousFilter>());
			}
			else
			{
				services.AddMvc();
			}

			var environmentConfiguration = new EnvironmentConfiguration(configuration);
			services.TryAddSingleton<IEnvironmentConfiguration>(environmentConfiguration);

			services.TryAddSingleton<IHTTPService, HTTPService>();
			services.TryAddSingleton<IFNOLService, FNOLService>();
			services.TryAddSingleton<INGPService, NGPService>();

			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

			services.AddAuthentication(options =>
				{
					options.DefaultScheme = "Cookies";
					options.DefaultChallengeScheme = "oidc";
				})
				.AddCookie("Cookies")
				.AddOpenIdConnect("oidc", options =>
				{
					options.SignInScheme = "Cookies";

					options.Authority = environmentConfiguration.ISEndpoint;

					options.ClientId = "gofnol";
					options.ResponseType = "id_token";
					options.SaveTokens = true;
				});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public virtual void Configure(IApplicationBuilder app)
		{
			if (hostingEnvironment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/FNOL/Error");
			}

			app.Use(async (context, next) =>
			{
				// In PCF all traffic beyond GoRouter goes over http. Need to make it look like it's https.
				if (hostingEnvironment.IsProduction())
				{
					context.Request.Scheme = "https";
				}

				await next.Invoke();
			});

			app.UseAuthentication();

			app.UseStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=FNOL}/{action=Index}/{id?}");
			});
		}
	}
}