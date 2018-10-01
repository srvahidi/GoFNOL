using System.Threading.Tasks;
using GoFNOL.Models;

namespace GoFNOL.Services
{
	public interface IFNOLService
	{
		string FormattedLossDate { get; }

		Task<Claim> CreateAssignment(FNOLRequest fnolRequest);
	}
}