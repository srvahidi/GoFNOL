using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace GoFNOL.tests
{
	public abstract class TestServerFixture : IDisposable
	{
		private readonly CustomWebApplicationFactory<TestStartup> _factory;

		protected HttpClient Client;

		protected TestServerFixture()
		{
			_factory = new CustomWebApplicationFactory<TestStartup>();
			Client = _factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(RegisterServices)).CreateClient(new WebApplicationFactoryClientOptions
			{
				AllowAutoRedirect = false
			});
		}

		public void Dispose()
		{
			_factory?.Dispose();
		}

		protected T GetService<T>() => _factory.Factories[0].Server.Host.Services.GetService<T>();

		protected virtual void RegisterServices(IServiceCollection customServices)
		{
		}
	}
}