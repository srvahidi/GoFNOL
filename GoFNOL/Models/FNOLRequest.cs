namespace GoFNOL.Models
{
	public class FNOLRequest
	{
		public string ClaimNumber { get; set; }

		public string MobileFlowIndicator { get; set; }

		public string VIN { get; set; }

		public string LossType { get; set; }

		public string Deductible { get; set; }

		public Party Owner { get; set; }
	}
}