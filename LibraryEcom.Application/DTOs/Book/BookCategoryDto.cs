namespace LibraryEcom.Application.DTOs.Book;

public class BookCategoryDto
{
    public Guid Id { get; set; }
    
    public string Title { get; set; }

    public bool IsBestseller { get; set; }
    
    public bool IsAwardWinner { get; set; }
    
    public DateTime PublishedDate { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public bool IsComingSoon { get; set; }  
    
    public bool IsDiscounted { get; set; }
    
    public decimal? DiscountPrice { get; set; }

}
