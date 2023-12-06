using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Real_Estate_Application.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        [Required]
        public string CustomerName { get; set; } = default!;
        public string Phone { get; set; } = default!;
        [Required, StringLength(100)]
        public string Email { get; set; } = default!;
        [Required, Column(TypeName = "date")]
        public DateTime? MoveInDate { get; set; }
        [Required, ForeignKey("Property")]
        public int PropertyId { get; set; }
        // add reference to the parent Property
        public virtual Property? Property { get; set; }
    }
}
