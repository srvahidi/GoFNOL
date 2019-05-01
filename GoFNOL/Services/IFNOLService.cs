using System.Threading.Tasks;
using GoFNOL.Models;

namespace GoFNOL.Services
{
	public interface IFNOLService
	{
		Task<FNOLResponse> CreateAssignment(FNOLRequest fnolRequest, string orgId);
	}
}