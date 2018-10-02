namespace GoFNOL.Models
{
	/// <summary>
	/// Model used to represent a First Notice Of Loss; used to create an EAIRequest
	/// </summary>
	public class FNOLRequest
	{
		/// <summary>
		/// The claim number to use when creating a Claim
		///
		/// <example>"14-ABCDE-01"</example>
		/// </summary>
		public string ClaimNumber { get; set; }

		/// <summary>
		/// The mobile flow indicator to use when creating a Claim
		///
		/// <example>""</example>
		/// </summary>
		public string MobileFlowIndicator { get; set; }

		/// <summary>
		/// The VIN to use when creating a Claim
		///
		/// <example>"1G1ZE5ST2HF172416"</example>
		/// </summary>
		public string VIN { get; set; }

		/// <summary>
		/// The loss type to use when creating a Claim
		///
		/// <example>""</example>
		/// </summary>
		public string LossType { get; set; }

		/// <summary>
		/// The deductible to use when creating a Claim
		///
		/// <example>""</example>
		/// </summary>
		public string Deductible { get; set; }

		/// <summary>
		/// The owner information to use when creating a Claim
		/// </summary>
		public Party Owner { get; set; }

		/// <summary>
		/// The profile ID used to create a claim
		///
		/// <example>"4774PE200001"</example>
		/// </summary>
		public string CreatedForProfileId { get; set; }
	}
}