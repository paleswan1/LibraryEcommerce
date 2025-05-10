using System.Net;
using LibraryCom.Controllers.Base;
using LibraryEcom.Application.Common.Response;
using LibraryEcom.Application.DTOs.Review;
using LibraryEcom.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryCom.Controllers;

[Route("api/review")]
public class ReviewController(IReviewService reviewService) : BaseController<ReviewController>
{
    [HttpGet("book/{bookId:guid}")]
    public IActionResult GetAllByBook(Guid bookId, int pageNumber , int pageSize , string? search = null)
    {
        var reviews = reviewService.GetAll(bookId, pageNumber, pageSize, out var rowCount, search);

        return Ok(new CollectionDto<ReviewDto>(reviews, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Reviews retrieved successfully.",
            Result = reviews
        });
    }

    [HttpGet("book/{bookId:guid}/all")]
    public IActionResult GetAllByBook(Guid bookId)
    {
        var reviews = reviewService.GetAll(bookId);

        return Ok(new ResponseDto<List<ReviewDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "All reviews for the book retrieved.",
            Result = reviews
        });
    }

    [HttpGet("{reviewId:guid}")]
    public IActionResult GetById(Guid reviewId)
    {
        var review = reviewService.GetById(reviewId);

        return Ok(new ResponseDto<ReviewDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Review retrieved successfully.",
            Result = review
        });
    }
    
    [HttpGet("my-review")]
    public IActionResult GetMyReview([FromQuery] Guid bookId)
    {
        var review = reviewService.GetUserReviewByBook(bookId);

        if (review == null)
        {
            return Ok(new ResponseDto<ReviewDto?>
            {
                StatusCode = (int)HttpStatusCode.NoContent,
                Message = "No review found for this user.",
                Result = null
            });
        }

        return Ok(new ResponseDto<ReviewDto>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "User review retrieved successfully.",
            Result = review
        });
    }


    [HttpPost]
    public IActionResult CreateReview([FromForm] CreateReviewDto review)
    {
        reviewService.Create(review);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Review created successfully.",
            Result = true
        });
    }

    [HttpPut("{reviewId:guid}")]
    public IActionResult UpdateReview(Guid reviewId, UpdateReviewDto review)
    {
        reviewService.Update(reviewId, review);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Review updated successfully.",
            Result = true
        });
    }

    [HttpDelete("{reviewId:guid}")]
    public IActionResult DeleteReview(Guid reviewId)
    {
        reviewService.Delete(reviewId);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Review deleted successfully.",
            Result = true
        });
    }
}
