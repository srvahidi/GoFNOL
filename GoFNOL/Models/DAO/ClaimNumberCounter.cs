using MongoDB.Bson.Serialization.Attributes;

namespace GoFNOL.Models.DAO
{
	[BsonIgnoreExtraElements]
	public class ClaimNumberCounter
	{
		[BsonId]
		public string OrgId { get; set; }

		public int Value { get; set; }
	}
}
