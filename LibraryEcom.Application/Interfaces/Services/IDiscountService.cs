using LibraryEcom.Application.Common.Service;
using LibraryEcom.Application.DTOs.Discount;
using LibraryEcom.Application.DTOs.Discounts;

namespace LibraryEcom.Application.Interfaces.Services
{
    public interface IDiscountService : ITransientService
    {
        List<DiscountDto> GetAll(int pageNumber, int pageSize, out int rowCount, string? search = null);

        DiscountDto? GetById(Guid id);

        void Create(CreateDiscountDto dto);

        void Update(Guid id, UpdateDiscountDto dto);

        void Delete(Guid id);

        List<DiscountDto> GetActiveDiscounts();

        void ToggleSaleFlag(Guid id, bool isSale); // Optional for Admin to flag/unflag as "On Sale"
    }
}