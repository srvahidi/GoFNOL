using System.Threading.Tasks;
using GoFNOL.Models;

namespace GoFNOL.Services
{
	public interface IFNOLService
	{
		Task<Claim> CreateAssignment(FNOLRequest fnolRequest);
	}
}