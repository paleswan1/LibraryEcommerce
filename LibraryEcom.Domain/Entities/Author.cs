using LibraryEcom.Domain.Common.Base;

namespace LibraryEcom.Domain.Entities;

public class Author: BaseEntity<Guid>
{
    public string Name { get; set; }
    
    public string Biography { get; set; }

    public DateOnly BirthDate { get; set; }
    
}