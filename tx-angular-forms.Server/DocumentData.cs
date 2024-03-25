using Microsoft.Extensions.Primitives;
using System.Text.Json.Serialization;

namespace tx_angular_forms.Server
{
	public class DocumentData
	{
		public string Name { get; set; }
		public string Document { get; set; }
	}

	public class CustomerData
	{
		public string Name { get; set; }
		public string FirstName { get; set; }
		public string Street { get; set; }
		public string City { get; set; }
		public string Zip { get; set; }
		public string Country { get; set; }
	}
}
