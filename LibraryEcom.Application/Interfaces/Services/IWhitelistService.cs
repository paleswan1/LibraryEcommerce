using LibraryEcom.Application.DTOs.Whitelist;

namespace LibraryEcom.Application.Interfaces.Services;

public interface IWhitelistService
{
    List<WhiteListDto> GetUserWishlist();

    List<WhiteListDto> GetUserWishlist(int pageNumber, int pageSize, out int rowCount, string? search = null);

    void AddToWishlist(CreateWhiteListDto dto);

    void RemoveFromWishlist(Guid whiteListId);

    bool IsBookWishlisted(Guid bookId);
}