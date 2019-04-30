using System.Threading.Tasks;
using GoFNOL.Models.DAO;
using GoFNOL.Persistence;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace GoFNOL.Outside.Repositories
{
	public class ClaimNumberCounterRepository : IClaimNumberCounterRepository
	{
		private readonly IMongoCollection<ClaimNumberCounter> _collection;

		private readonly ILogger<ClaimNumberCounterRepository> _logger;

		public ClaimNumberCounterRepository(IMongoConnection mongoConnection,
			ILogger<ClaimNumberCounterRepository> logger)
		{
			_collection = mongoConnection.GetCollection<ClaimNumberCounter>();

			_logger = logger;
		}

		public async Task<int> IncrementCounter(string orgId)
		{
			var filter = new FilterDefinitionBuilder<ClaimNumberCounter>().Where(c => c.OrgId == orgId);
			var update = new UpdateDefinitionBuilder<ClaimNumberCounter>().Inc(c => c.Value, 1);
			var options = new FindOneAndUpdateOptions<ClaimNumberCounter>
				{IsUpsert = true, ReturnDocument = ReturnDocument.After};
			var counter = await _collection.FindOneAndUpdateAsync(filter, update, options);
			return counter.Value;
		}
	}
}