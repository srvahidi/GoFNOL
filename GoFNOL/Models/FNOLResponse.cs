namespace GoFNOL.Models
{
	public class FNOLResponse
	{
		public FNOLResponse(string workAssignmentId)
		{
			WorkAssignmentId = workAssignmentId;
		}

		public string WorkAssignmentId { get; }
	}
}