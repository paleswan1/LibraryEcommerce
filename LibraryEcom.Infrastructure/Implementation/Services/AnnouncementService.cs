using LibraryEcom.Application.DTOs.Announcement;
using LibraryEcom.Application.Exceptions;
using LibraryEcom.Application.Interfaces.Repositories.Base;
using LibraryEcom.Application.Interfaces.Services;
using LibraryEcom.Domain.Entities;

namespace LibraryEcom.Infrastructure.Implementation.Services;

public class AnnouncementService(IGenericRepository genericRepository): IAnnouncementService
{
    public List<AnnouncementDto> GetAll(int pageNumber, int pageSize, out int rowCount, string? search = null)
    {
        var annoucements = genericRepository.GetPagedResult<Announcement>(pageNumber, pageSize, out rowCount
        , x => (string.IsNullOrEmpty(search) || x.Message.ToLower().Contains(search.ToLower()) )).ToList();
        
        var announcementDtos = new List<AnnouncementDto>();

        foreach (var announce in annoucements)
        {
            announcementDtos.Add(new AnnouncementDto
            {
                Id = announce.Id,
                Message = announce.Message,
                StartDate = announce.StartDate,
                EndDate = announce.EndDate
            });
        }

        return announcementDtos;

        
    }

    public List<AnnouncementDto> GetAll(string? search = null)
    {
        var annoucements = genericRepository
            .Get<Announcement>(x => (string.IsNullOrEmpty(search) || x.Message.ToLower().Contains(search.ToLower())))
            .ToList();
        
        var announcementDtos = new List<AnnouncementDto>();

        foreach (var announce in annoucements)
        {
            announcementDtos.Add(new AnnouncementDto
            {
                Id = announce.Id,
                Message = announce.Message,
                StartDate = announce.StartDate,
                EndDate = announce.EndDate
            });
        }

        return announcementDtos;
    }

    public AnnouncementDto? GetById(Guid id)
    {
        var announcement = genericRepository.GetById<Announcement>(id)
            ?? throw new NotFoundException("The following entity was not found");

        var result = new AnnouncementDto()
        {
            Id = announcement.Id,
            Message = announcement.Message,
            StartDate = announcement.StartDate,
            EndDate = announcement.EndDate
        };
        return result;
    }

    public void Create(CreateAnnouncementDto dto)
    {
        var existing = genericRepository.GetFirstOrDefault<Announcement>(x =>
            x.Message.ToLower() == dto.Message.ToLower() &&
            x.StartDate == dto.StartDate &&
            x.EndDate == dto.EndDate);

        if (existing != null)
        {
            throw new NotFoundException("An identical announcement already exists.");
        }

        var model = new Announcement
        {
            Message = dto.Message,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };

        genericRepository.Insert(model);
    }

    public void Update(Guid id, UpdateAnnouncementDto dto)
    {
        var model = genericRepository.GetById<Announcement>(id)
                    ?? throw new NotFoundException("The following announcement with specified Id was not found.");

        model.Message = dto.Message;
        model.StartDate = dto.StartDate;
        model.EndDate = dto.EndDate;

        genericRepository.Update(model);
    }

    public void Delete(Guid id)
    {
        var announcement = genericRepository.GetById<Announcement>(id)
                           ?? throw new NotFoundException("The following announcement with specified Id was not found.");

        genericRepository.Delete(announcement);
    }
}