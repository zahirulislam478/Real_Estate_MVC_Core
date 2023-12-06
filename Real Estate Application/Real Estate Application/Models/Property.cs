using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Real_Estate_Application.Models
{
    public enum PropertyType
    {
        Residential = 1,
        Commercial,
        Industrial
    }
    public class Property
    {
        public int PropertyId { get; set; }
        [Required, StringLength(100)]
        public string Address { get; set; } = default!;
        [EnumDataType(typeof(PropertyType))]
        public PropertyType Type { get; set; }
        [Required]
        public int? Bedrooms { get; set; } = default!;
        [Required]
        public int? Bathrooms { get; set; } = default!;
        [Required]
        public double? SquareFootage { get; set; } = default!;
        [Required, Column(TypeName = "money")]
        public decimal? Price { get; set; }
        [Required, StringLength(100)]
        public string Picture { get; set; } = default!;
        public bool IsAvailable { get; set; }
        // create child ref to Customer model (1 Property / Many Customers)
        public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}
