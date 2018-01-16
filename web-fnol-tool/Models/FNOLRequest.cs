namespace web_fnol_tool.Models
{
    public class FNOLRequest
    {
        public string ClaimNumber { get; set; }

        public string VIN { get; set; }

        public string LossType { get; set; }

        public string Deductible { get; set; }

        public Party Owner { get; set; }
    }

    public class Party
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public Address Address { get; set; }
    }

    public class Address
    {
        public string PostalCode { get; set; }

        public string State { get; set; }
    }
}