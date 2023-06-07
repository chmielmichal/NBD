using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NBD.Models
{
    public class ComputerModel
    {
		[BsonRepresentation(BsonType.ObjectId)]
		public string? Id { get; set; }

		[Display(Name = "Computer Name")]
		public string? Name { get; set; }
		[Display(Name = "Creation Year")]
		public int? Year { get; set; }
		public string? ImageId { get; set; }

		public bool HasImage() => !string.IsNullOrWhiteSpace(ImageId);
	}
}
