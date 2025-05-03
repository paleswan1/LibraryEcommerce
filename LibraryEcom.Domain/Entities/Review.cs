using System.ComponentModel.DataAnnotations.Schema;
using LibraryEcom.Domain.Common.Base;
using LibraryEcom.Domain.Entities.Identity;

namespace LibraryEcom.Domain.Entities;

public class Review: BaseEntity<Guid>
{
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    
    [ForeignKey(nameof(Book))]
    public Guid BookId { get; set; }
    
    public int Rating { get; set; } 
    
    public string Comment { get; set; }
    
    public DateTime ReviewDate { get; set; }
    
    public virtual User User { get; set; }
    
    public virtual Book Book { get; set; }
}