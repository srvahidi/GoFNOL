using System.IdentityModel.Tokens.Jwt;
using GoFNOL.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
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

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			var environmentConfiguration = new EnvironmentConfiguration(configuration);
			services.TryAddSingleton<IEnvironmentConfiguration>(environmentConfiguration);
			services.TryAddSingleton<IHTTPService, HTTPService>();
			services.TryAddSingleton<IFNOLService, FNOLService>();
			services.TryAddSingleton<INGPService, NGPService>();
			services.TryAddScoped<IMongoService, MongoService>();
			services.TryAddScoped<ClaimNumberService>();

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

			// In production, the React files will be served from this directory
			services.AddSpaStaticFiles(c => c.RootPath = "ClientApp/build");
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public virtual void Configure(IApplicationBuilder app)
		{
			Configure(app, true);
		}

		protected void Configure(IApplicationBuilder app, bool useSpa)
		{
			if (hostingEnvironment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
			}

			app.UseMiddleware<HealthCheckMiddleware>();

			if (hostingEnvironment.IsProduction())
			{
				app.UseAuthentication();
			}
			else
			{
				// Allow developers to skip IS and have always signed in user.
				app.UseMiddleware<UserSpoofMiddleware>();
			}

			app.Use(async (context, next) =>
			{
				// In PCF all traffic beyond GoRouter goes over http. Need to make it look like it's https.
				if (hostingEnvironment.IsProduction())
				{
					context.Request.Scheme = "https";
				}

				// Force authorization somehow finally in that weird SPA setup
				if (hostingEnvironment.IsProduction() && !context.User.Identity.IsAuthenticated)
				{
					await context.ChallengeAsync();
				}
				else
				{
					await next.Invoke();
				}
			});

			app.UseStaticFiles();

			app.UseSpaStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller}/{action=Index}/{id?}");
			});

			if (useSpa)
			{
				app.UseSpa(spa =>
				{
					spa.Options.SourcePath = "ClientApp";

					if (hostingEnvironment.IsDevelopment())
					{
						spa.UseReactDevelopmentServer(npmScript: "start");
					}
				});
			}
		}
	}
}