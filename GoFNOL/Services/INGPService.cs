using System.Threading.Tasks;

namespace GoFNOL.Services
{
	public interface INGPService
	{
		Task<string> GetUserProfileIdAsync(string userUid);
	}
}