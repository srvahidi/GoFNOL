using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GoFNOL.Services
{
	public interface IHTTPService
	{
		Task<HttpResponseMessage> GetAsync(Uri uri);

		Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content);

		Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content);
	}

	public class HTTPService : IHTTPService
	{
		private readonly HttpClient httpClient = new HttpClient();

		public Task<HttpResponseMessage> GetAsync(Uri uri)
		{
			return httpClient.GetAsync(uri);
		}

		public Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content)
		{
			return httpClient.PostAsync(uri, content);
		}

		public Task<HttpResponseMessage> PutAsync(Uri uri, HttpContent content)
		{
			return httpClient.PutAsync(uri, content);
		}
	}
}