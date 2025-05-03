using System.ComponentModel.DataAnnotations.Schema;
using LibraryEcom.Domain.Common.Base;
using LibraryEcom.Domain.Entities.Identity;

namespace LibraryEcom.Domain.Entities;

public class CartItem: BaseEntity<Guid>
{
    [ForeignKey(nameof(Cart))]
    public Guid CartId { get; set; }
    
    [ForeignKey(nameof(Book))]
    public Guid BookId { get; set; }
    
    public int Quantity { get; set; }
    
    public DateTime AddedAt { get; set; }

    public virtual Cart Cart { get; set; }
    
    public virtual Book Book { get; set; }
}