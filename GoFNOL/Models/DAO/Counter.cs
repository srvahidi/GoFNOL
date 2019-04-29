using MongoDB.Bson.Serialization.Attributes;

namespace GoFNOL.Models.DAO
{
	[BsonIgnoreExtraElements]
	public class Counter
	{
		public int Value { get; set; }

		[BsonId]
		public string Name { get; set; }

		public string OrgId { get; set; }
	}
}
