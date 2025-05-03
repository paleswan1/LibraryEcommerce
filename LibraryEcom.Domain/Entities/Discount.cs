using System.ComponentModel.DataAnnotations.Schema;
using LibraryEcom.Domain.Common.Base;

namespace LibraryEcom.Domain.Entities;

public class Discount: BaseEntity<Guid>
{
    [ForeignKey(nameof(Book))]
    public Guid BookId { get; set; }
    
    public decimal DiscountPercentage { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public bool IsActive { get; set; }
    
    public bool IsSaleFlag { get; set; }
    
    public virtual Book Book { get; set; }
    
}