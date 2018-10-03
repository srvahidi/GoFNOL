using System.Threading.Tasks;
using GoFNOL.Models;

namespace GoFNOL.Services
{
	public interface IFNOLService
	{
		string FormattedLossDate { get; }

		Task<FNOLResponse> CreateAssignment(FNOLRequest fnolRequest);
	}
}