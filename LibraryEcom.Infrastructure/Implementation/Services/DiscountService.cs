using LibraryEcom.Application.DTOs.Discount;
using LibraryEcom.Application.DTOs.Discounts;
using LibraryEcom.Application.Exceptions;
using LibraryEcom.Application.Interfaces.Repositories.Base;
using LibraryEcom.Application.Interfaces.Services;
using LibraryEcom.Domain.Entities;

namespace LibraryEcom.Infrastructure.Implementation.Services;

public class DiscountService(IGenericRepository genericRepository) : IDiscountService
{
    public List<DiscountDto> GetAll(int pageNumber, int pageSize, out int rowCount, string? search = null)
    {
        var discounts = genericRepository.GetPagedResult<Discount>(pageNumber, pageSize, out rowCount,
                x => string.IsNullOrEmpty(search) || x.Book.Title.ToLower().Contains(search.ToLower()))
            .ToList();

        var bookIds = discounts.Select(d => d.BookId).Distinct().ToList();

        var discountDtos = new List<DiscountDto>();
        
        var books = genericRepository.Get<Book>(b => bookIds.Contains(b.Id)).ToList();


        foreach (var discount in discounts)
        {
            discountDtos.Add(new DiscountDto
            {
                Id = discount.Id,
                BookId = discount.BookId,
                DiscountPercentage = discount.DiscountPercentage,
                StartDate = discount.StartDate,
                IsSaleFlag = discount.IsSaleFlag,
                EndDate = discount.EndDate,
                BookTitle = books.FirstOrDefault(b => b.Id == discount.BookId)?.Title,
            });
        }

        return discountDtos;
    }

    public DiscountDto? GetById(Guid id)
    {
        var discount = genericRepository.GetById<Discount>(id)
                       ?? throw new NotFoundException("The discount with the specified ID was not found.");

        return new DiscountDto
        {
            Id = discount.Id,
            BookId = discount.BookId,
            DiscountPercentage = discount.DiscountPercentage,
            StartDate = discount.StartDate,
            EndDate = discount.EndDate,
            IsSaleFlag = discount.IsSaleFlag,
            
        };
    }

    public void Create(CreateDiscountDto dto)
    {
        var existing = genericRepository.GetFirstOrDefault<Discount>(x =>
            x.BookId == dto.BookId &&
            x.StartDate == dto.StartDate &&
            x.EndDate == dto.EndDate);

        if (existing != null)
        {
            throw new NotFoundException("A discount for this book and date range already exists.");
        }

        var model = new Discount
        {
            BookId = dto.BookId,
            DiscountPercentage = dto.DiscountPercentage,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsActive = dto.IsActive,
            IsSaleFlag = dto.IsSaleFlag
        };

        genericRepository.Insert(model);
    }

    public void Update(Guid id, UpdateDiscountDto dto)
    {
        var discount = genericRepository.GetById<Discount>(id)
                       ?? throw new NotFoundException("The discount with the specified ID was not found.");

        discount.DiscountPercentage = dto.DiscountPercentage;
        discount.StartDate = dto.StartDate;
        discount.EndDate = dto.EndDate;
        discount.IsActive = dto.IsActive;
        discount.IsSaleFlag = dto.IsSaleFlag;

        genericRepository.Update(discount);
    }

    public void Delete(Guid id)
    {
        var discount = genericRepository.GetById<Discount>(id)
                       ?? throw new NotFoundException("The discount with the specified ID was not found.");

        genericRepository.Delete(discount);
    }

    public List<DiscountDto> GetActiveDiscounts()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var discounts = genericRepository
            .Get<Discount>(x => x.IsActive && x.StartDate <= today && x.EndDate >= today)
            .ToList();


        return discounts.Select(d => new DiscountDto
        {
            Id = d.Id,
            BookId = d.BookId,
            DiscountPercentage = d.DiscountPercentage,
            StartDate = d.StartDate,
            EndDate = d.EndDate,
            IsSaleFlag = d.IsSaleFlag
        }).ToList();
    }

    public void ToggleSaleFlag(Guid id, bool isSale)
    {
        var discount = genericRepository.GetById<Discount>(id)
                       ?? throw new NotFoundException("The discount with the specified ID was not found.");

        discount.IsSaleFlag = isSale;
        genericRepository.Update(discount);
    }
}