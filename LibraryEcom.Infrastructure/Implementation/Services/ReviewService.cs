using LibraryEcom.Application.DTOs.Review;
using LibraryEcom.Application.DTOs.User;
using LibraryEcom.Application.Exceptions;
using LibraryEcom.Application.Interfaces;
using LibraryEcom.Application.Interfaces.Repositories.Base;
using LibraryEcom.Domain.Entities;
using LibraryEcom.Domain.Entities.Identity;

namespace LibraryEcom.Infrastructure.Implementation.Services;

public class ReviewService(IGenericRepository genericRepository): IReviewService
{
    public List<ReviewDto> GetAll(Guid bookId, int pageNumber, int pageSize, out int rowCount, string? search = null)
    {
        var reviews = genericRepository.GetPagedResult<Review>(pageNumber, pageSize, out rowCount,
            x => x.BookId == bookId ).ToList(); 
        
        var reviewDtos = new List<ReviewDto>();

        foreach (var review in reviews)
        {
            var user = genericRepository.GetById<User>(review.UserId) ?? throw new NotFoundException("User not found.");
            reviewDtos.Add(new ReviewDto
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                ReviewDate = review.ReviewDate,
                User = new UserDto()
                {
                    Id = user.Id,
                    Address = user.Address,
                    Name = user.Name,
                    Gender = user.Gender,
                    EmailAddress = user.Email,
                    IsActive = user.IsActive,
                    RegisteredDate = user.RegisteredDate,
                    ImageURL = user.ImageURL,
                }
            });
        }

        return reviewDtos;    
    }

    public List<ReviewDto> GetAll(Guid bookId)
    {
        var reviews = genericRepository.Get<Review>(x => x.BookId == bookId ).ToList();

        var reviewDtos = new List<ReviewDto>();
        
        foreach (var review in reviews)
        {
            var user = genericRepository.GetById<User>(review.UserId) ?? throw new NotFoundException("User not found.");
            reviewDtos.Add(new ReviewDto
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                ReviewDate = review.ReviewDate,
                User = new UserDto()
                {
                    Id = user.Id,
                    Address = user.Address,
                    Name = user.Name,
                    Gender = user.Gender,
                    EmailAddress = user.Email,
                    IsActive = user.IsActive,
                    RegisteredDate = user.RegisteredDate,
                    ImageURL = user.ImageURL,
                }
            });
        }

        return reviewDtos;
        
    }
    
    public ReviewDto GetById(Guid id)
    {
        var review = genericRepository.GetById<Review>(id) ?? throw new NotFoundException("Review not found.");
        var user = genericRepository.GetById<User>(review.UserId) ?? throw new NotFoundException("User not found.");
        
        return new ReviewDto
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            ReviewDate = review.ReviewDate,
            User = new UserDto()
            {
                Id = user.Id,
                Address = user.Address,
                Name = user.Name,
                Gender = user.Gender,
                EmailAddress = user.Email,
                IsActive = user.IsActive,
                RegisteredDate = user.RegisteredDate,
                ImageURL = user.ImageURL
            }
        };    
    }

   
    public void Create(CreateReviewDto dto)
    {
        var book = genericRepository.GetById<Book>(dto.BookId)
            ?? throw new NotFoundException("Book not found.");
        
        var review = new Review
        {
            BookId = book.Id,
            Rating = dto.Rating,
            Comment = dto.Comment,
            ReviewDate = DateTime.UtcNow
        };
        genericRepository.Insert(review);
    }

    public void Update(Guid id, UpdateReviewDto dto)
    {
        var review = genericRepository.GetById<Review>(id)
                    ?? throw new NotFoundException("Review not found");
        
        review.Comment = dto.Comment;
        review.Rating = dto.Rating;
        
        genericRepository.Update(review);
    }

    public void Delete(Guid id)
    {
        var review = genericRepository.GetById<Review>(id)
                    ?? throw new NotFoundException("Review not found");
        
        genericRepository.Delete(review);
    }
}