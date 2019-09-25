using System.IdentityModel.Tokens.Jwt;
using GoFNOL.Outside.Repositories;
using GoFNOL.Persistence;
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

namespace GoFNOL
{
	public class Startup
	{
		private readonly IConfiguration _configuration;

		private IEnvironmentConfiguration _environmentConfiguration;

		public Startup(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			_environmentConfiguration = new EnvironmentConfiguration(_configuration);

			services.AddMvc(config =>
			{
				if (!_environmentConfiguration.DisableAuth)
				{
					config.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
				}
			}).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			services.AddSingleton<IEnvironmentConfiguration>(_environmentConfiguration);
			services.AddSingleton<IHTTPService, HTTPService>();
			services.AddScoped<IFNOLService, FNOLService>();
			services.AddSingleton<INGPService, NGPService>();
			services.AddScoped<IMongoConnection, MongoConnection>();
			services.AddScoped<IClaimNumberCounterRepository, ClaimNumberCounterRepository>();

			if (!_environmentConfiguration.DisableAuth)
			{
				JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
				services.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				}).AddJwtBearer(o =>
				{
					o.Audience = "gofnol.api";
					o.Authority = _environmentConfiguration.ISEndpoint;
				});
			}

			services.AddSpaStaticFiles(c => c.RootPath = "ClientApp/build");

			services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info {Title = "GoFNOL", Version = "v1"}));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public virtual void Configure(IApplicationBuilder app, IHostingEnvironment hostingEnvironment)
		{
			Configure(app, hostingEnvironment, true);
		}

		protected void Configure(IApplicationBuilder app, IHostingEnvironment hostingEnvironment, bool useSpa)
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

			app.UseMiddleware<VersionMiddleware>();

			app.UseStaticFiles();

			app.UseSpaStaticFiles();

			app.UseAuthentication();

			app.UseMvc(routes => routes.MapRoute("default", "{controller}/{action=Index}/{id?}"));

			app.UseSwagger();

			app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GoFNOL"));

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