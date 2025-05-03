namespace LibraryEcom.Application.DTOs.Review;

public class CreateReviewDto
{

    public Guid BookId { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;
}