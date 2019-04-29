using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoFNOL.Models
{
	public class ClaimNumberCounter
	{
		public ClaimNumberCounter(string orgId, int value)
		{
			OrgId = orgId;
			Value = value;
		}

		public string OrgId { get; }

		public int Value { get; }
	}
}
