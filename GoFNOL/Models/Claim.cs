namespace GoFNOL.Models
{
	/// <summary>
	/// Model used to represent a Claim returned from EAI
	/// </summary>
	public class Claim
	{
		/// <summary>
		/// The claim number of the claim created
		///
		/// <example>"14-ABCDE-01"</example>
		/// <remarks>XPath Selector: "//ADP_FNOL_ASGN_INPUT/CLAIM/CLAIM_NBR"</remarks>
		/// </summary>
		public string ClaimNumber { get; set; }

		/// <summary>
		/// The profile ID used when the claim was created
		///
		/// <example>"4774PE200001"</example>
		/// <remarks>XPath Selector: "//ADP_FNOL_ASGN_INPUT/ASSIGNED_TO/USER_ID"</remarks>
		/// </summary>
		public string CreatedForProfileId { get; set; }

		/// <summary>
		/// The work assignment ID of the claim created
		///
		/// <example>"1234567"</example>
		/// <remarks>XPath Selector: "//ADP_TRANSACTION_ID"</remarks>
		/// </summary>
		public string WorkAssignmentId { get; set; }
	}
}
