using System.ComponentModel.DataAnnotations.Schema;
using LibraryEcom.Domain.Common.Base;

namespace LibraryEcom.Domain.Entities;

public class BookAuthor: BaseEntity<Guid>
{
    [ForeignKey(nameof(Book))]
    public Guid BookId { get; set; }
    
    [ForeignKey(nameof(Author))]
    public Guid AuthorId { get; set; }
    
    public virtual Book Book { get; set; }
    
    public virtual Author Author { get; set; }
    
}