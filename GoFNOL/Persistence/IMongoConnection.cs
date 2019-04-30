using MongoDB.Driver;

namespace GoFNOL.Persistence
{
	public interface IMongoConnection
	{
		string Owner { get; set; }

		IMongoCollection<T> GetCollection<T>();
	}
}