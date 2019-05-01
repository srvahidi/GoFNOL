using MongoDB.Driver;

namespace GoFNOL.Persistence
{
	public class MongoConnection : IMongoConnection
	{
		protected readonly IMongoDatabase Database;

		public string Owner { get; set; }

		public MongoConnection(IEnvironmentConfiguration envConfig)
		{
			var mongoUrl = new MongoUrl(envConfig.DbConnectionString);
			Database = new MongoClient(mongoUrl).GetDatabase(mongoUrl.DatabaseName);
		}

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
