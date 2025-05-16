using LibraryEcom.Application.Common.Service;
using LibraryEcom.Application.DTOs.Announcement;

namespace LibraryEcom.Application.Interfaces.Services;

public interface IAnnouncementService: ITransientService
{
   List<AnnouncementDto> GetAll(int pageNumber, int pageSize, out int rowCount, string? search = null);
    
   List<AnnouncementDto> GetAll(string? search = null);
    
   AnnouncementDto? GetById(Guid id);

   void Create(CreateAnnouncementDto dto);

   void Update(Guid id, UpdateAnnouncementDto dto);

   void Delete(Guid id);
}