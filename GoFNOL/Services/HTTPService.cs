using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GoFNOL.Services
{
	public interface IHTTPService
	{
		Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content);
	}

	public class HTTPService : IHTTPService
	{
		private readonly HttpClient httpClient = new HttpClient();

		public Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content)
		{
			return httpClient.PostAsync(uri, content);
		}
	}
}