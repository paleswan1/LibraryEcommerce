using LibraryEcom.Application.Common.Service;
using LibraryEcom.Application.DTOs.Review;

namespace LibraryEcom.Application.Interfaces;

public interface IReviewService: ITransientService
{
    List<ReviewDto> GetAll(Guid bookId, int pageNumber, int pageSize, out int rowCount, string? search = null);
    
    List<ReviewDto> GetAll(Guid bookId);
    
    ReviewDto GetById(Guid reviewId);
    
    ReviewDto? GetUserReviewByBook(Guid bookId);
    
    void Create(CreateReviewDto dto);
    
    void Update(Guid id, UpdateReviewDto dto);

    void Delete(Guid id);
}