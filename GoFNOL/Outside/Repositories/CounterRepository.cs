using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoFNOL.Models.DAO;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace GoFNOL.Outside.Repositories
{
	//public class CounterRepository : ICounterRepository
	//{
		//private readonly IMongoCollection<Counter> _collection;

		//private readonly ILogger<CounterRepository> _logger;

		//public CounterRepository(IMongoConnection mongoConnection, ILogger<CounterRepository> logger)
		//{
		//	//_collection = mongoConnection.GetCollection<Counter>();
		//	//_logger = logger;
		//}

		//public void Initialize<T>()
		//{
		//	var counter = new Counter
		//	{
		//		Name = GetEntityName<T>(),
		//		OrgId = "",
		//		Value = 0
		//	};

		//	try
		//	{
		//		_collection.InsertOne(counter);
		//		_logger.LogInformation($"Initialized counter for entity {counter.Name}");
		//	}
		//	catch (MongoWriteException)
		//	{
		//	}
		//}

		//public async Task<int> IncrementCounter<T>(int incValue, string orgId)
		//{
		//	var name = GetEntityName<T>();
		//	var counter = await _collection.FindOneAndUpdateAsync(c => c.Name == name,
		//		new UpdateDefinitionBuilder<Counter>().Inc(c => c.Value, incValue));
		//	_logger.LogInformation($"Incremented counter for org {orgId} and entity {counter.Name} to value {counter.Value}");
		//	return counter.Value + 1;
		//}

		//private static string GetEntityName<T>()
		//{
		//	var name = typeof(T).Name;
		//	if (name.EndsWith("DAO"))
		//	{
		//		return name.Substring(0, name.Length - 3);
		//	}

		//	return name;
		//}
	//}
}
