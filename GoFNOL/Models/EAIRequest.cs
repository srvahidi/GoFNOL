using System.Net.Http;
using System.Net.Http.Headers;

namespace GoFNOL.Models
{
	/// <inheritdoc />
	/// <summary>
	/// Model used to represent a request to EAI; used to create a Claim
	/// </summary>
	public class EAIRequest : StringContent
	{
		private const string SOAPHeaderName = "SOAPAction";

		private const string SOAPHeaderValue = "http://csg.adp.com/Transmit";

		private const string XMLContentType = "text/xml";

		public EAIRequest(string eaiRequestContent) : base(eaiRequestContent)
		{
			Headers.ContentType = MediaTypeHeaderValue.Parse(XMLContentType);
			Headers.Add(SOAPHeaderName, SOAPHeaderValue);
		}
	}
}
