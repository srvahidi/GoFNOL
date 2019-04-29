using System.Threading.Tasks;
using GoFNOL.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace GoFNOL.Services
{
	public class ClaimNumberService
	{
		private readonly IMongoService _mongoService;

		private readonly ILogger<ClaimNumberService> _logger;

		public ClaimNumberService(IMongoService mongoService, ILogger<ClaimNumberService> logger)
		{
			_mongoService = mongoService;
			_logger = logger;
		}

		public async Task<string> GetNextClaimNumberAsync()
		{
			_logger.LogInformation("Generating new claim number");
			var filter = new FilterDefinitionBuilder<ClaimNumberCounter>().Where(c => c.OrgId == _mongoService.Owner);
			var update = new UpdateDefinitionBuilder<ClaimNumberCounter>().Inc(c => c.Value, 1);
			var options = new FindOneAndUpdateOptions<ClaimNumberCounter> { IsUpsert = true };
			var previousCounter = await _mongoService.GetCollection<ClaimNumberCounter>().FindOneAndUpdateAsync(filter, update, options);
			_logger.LogInformation($"Previous counter value is {(previousCounter != null ? previousCounter.Value.ToString() : "null")}");
			var previousCounterValue = previousCounter?.Value ?? 0;
			var newClaimNumber = $"14-{_mongoService.Owner.ToUpper()}-{(previousCounterValue + 1).ToString().PadLeft(5, '0')}";
			_logger.LogInformation($"New claim number is {newClaimNumber}");

			return newClaimNumber;
		}
	}
}
