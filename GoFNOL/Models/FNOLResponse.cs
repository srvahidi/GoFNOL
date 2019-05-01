namespace GoFNOL.Models
{
	public class FNOLResponse
	{
		public FNOLResponse(string workAssignmentId, string claimNumber)
		{
			WorkAssignmentId = workAssignmentId;
			ClaimNumber = claimNumber;
		}

		public string ClaimNumber { get; }

		public string WorkAssignmentId { get; }
	}
}