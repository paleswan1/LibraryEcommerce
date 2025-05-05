using System.ComponentModel.DataAnnotations.Schema;
using LibraryEcom.Domain.Common.Base;

namespace LibraryEcom.Domain.Entities;

public class Discount: BaseEntity<Guid>
{
    [ForeignKey(nameof(Book))]
    public Guid BookId { get; set; }
    
    public decimal DiscountPercentage { get; set; }
    
    // [Column(TypeName = "timestamp without time zone")]
    public DateOnly StartDate { get; set; }

    // [Column(TypeName = "timestamp without time zone")]
    public DateOnly EndDate { get; set; }
    
    public bool IsSaleFlag { get; set; }
    
    public virtual Book Book { get; set; }
    
}