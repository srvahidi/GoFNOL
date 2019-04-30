using System.IdentityModel.Tokens.Jwt;
using GoFNOL.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
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
			services.AddMvc(config =>
			{
				config.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
			}).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			var environmentConfiguration = new EnvironmentConfiguration(configuration);
			services.TryAddSingleton<IEnvironmentConfiguration>(environmentConfiguration);
			services.TryAddSingleton<IHTTPService, HTTPService>();
			services.TryAddSingleton<IFNOLService, FNOLService>();
			services.TryAddSingleton<INGPService, NGPService>();

			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(o =>
			{
				o.Audience = "gofnol.api";
				o.Authority = environmentConfiguration.ISEndpoint;
			});

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

			app.Use(async (context, next) =>
			{
				// In PCF all traffic beyond GoRouter goes over http. Need to make it look like it's https.
				if (hostingEnvironment.IsProduction())
				{
					context.Request.Scheme = "https";
				}

				await next.Invoke();
			});

			app.UseStaticFiles();

			app.UseSpaStaticFiles();

			app.UseAuthentication();

			app.UseMvc(routes => routes.MapRoute("default", "{controller}/{action=Index}/{id?}"));

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