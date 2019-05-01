using System.Threading.Tasks;

namespace GoFNOL.Outside.Repositories
{
	public interface IClaimNumberCounterRepository
	{
		Task<int> IncrementCounter(string orgId);
	}
}