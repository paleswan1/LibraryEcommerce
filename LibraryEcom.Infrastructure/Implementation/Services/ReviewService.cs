using LibraryEcom.Application.Common.User;
using LibraryEcom.Application.DTOs.Review;
using LibraryEcom.Application.DTOs.User;
using LibraryEcom.Application.Exceptions;
using LibraryEcom.Application.Interfaces;
using LibraryEcom.Application.Interfaces.Repositories.Base;
using LibraryEcom.Domain.Entities;
using LibraryEcom.Domain.Entities.Identity;

namespace LibraryEcom.Infrastructure.Implementation.Services;

public class ReviewService(IGenericRepository genericRepository,
    ICurrentUserService currentUserService): IReviewService
{
    public List<ReviewDto> GetAll(Guid bookId, int pageNumber, int pageSize, out int rowCount, string? search = null)
    {
        var currentUserId = currentUserService.GetUserId;

        var reviews = genericRepository.GetPagedResult<Review>(pageNumber, pageSize, out rowCount,
            x => x.BookId == bookId).ToList();

        var reviewDtos = new List<ReviewDto>();

        foreach (var review in reviews)
        {
            var user = genericRepository.GetById<User>(review.UserId)
                       ?? throw new NotFoundException("User not found.");

            reviewDtos.Add(new ReviewDto
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                ReviewDate = review.ReviewDate,
                IsOwnReview = currentUserId == review.UserId, 
                User = new UserDto
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

    public ReviewDto? GetUserReviewByBook(Guid bookId)
    {
        var currentUserId = currentUserService.GetUserId;

        var review = genericRepository
            .Get<Review>(x => x.BookId == bookId && x.UserId == currentUserId)
            .FirstOrDefault();

        if (review == null) return null;

        var user = genericRepository.GetById<User>(currentUserId)
                   ?? throw new NotFoundException("User not found.");

        return new ReviewDto
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            ReviewDate = review.ReviewDate,
            IsOwnReview = true,
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                EmailAddress = user.Email,
                IsActive = true,
                RegisteredDate = user.RegisteredDate,
                ImageURL = user.ImageURL,
                Address = user.Address,
                Gender = user.Gender,
            }
        };
    }


    public void Create(CreateReviewDto dto)
    {
        var book = genericRepository.GetById<Book>(dto.BookId)
            ?? throw new NotFoundException("Book not found.");
        
        var user = genericRepository.GetById<User>(dto.UserId)
                   ?? throw new NotFoundException("User not found");
        
        var review = new Review
        {
            BookId = book.Id,
            UserId = user.Id,
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