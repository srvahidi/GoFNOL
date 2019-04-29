using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace GoFNOL.Services
{
	public class MongoService : IMongoService
	{
		protected readonly IMongoDatabase Database;

		public MongoService(IEnvironmentConfiguration envConfig)
		{
			var mongoUrl = new MongoUrl(envConfig.DbConnectionString);
			Database = new MongoClient(mongoUrl).GetDatabase(mongoUrl.DatabaseName);
		}

		public string Owner { get; set; }

		public IMongoCollection<T> GetCollection<T>()
		{
			var name = typeof(T).Name;
			if (name.EndsWith("DAO"))
			{
				name = name.Substring(0, name.Length - 3);
			}

			return Database.GetCollection<T>(name);
		}
	}
}
