using MongoDB.Driver;

namespace GoFNOL.Services
{
	public interface IMongoService
	{
		string Owner { get; set; }

		IMongoCollection<T> GetCollection<T>();
	}
}