using LibraryEcom.Application.DTOs.User;

namespace LibraryEcom.Application.DTOs.Review;

public class ReviewDto
{
    public Guid Id { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime ReviewDate { get; set; }
    
    public UserDto User { get; set; }
    
    public bool IsOwnReview { get; set; } 
}