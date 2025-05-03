using System.Net;
using LibraryEcom.Application.Common.Response;
using LibraryEcom.Application.DTOs.Announcement;
using LibraryEcom.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using LibraryCom.Controllers.Base;

namespace LibraryCom.Controllers;

[Route("api/announcements")]
public class AnnouncementController(IAnnouncementService announcementService): BaseController<AnnouncementController>
{

    [HttpGet]
    public IActionResult GetAnnouncements(int pageNumber, int pageSize, string? search)
    {
        var announcements = announcementService.GetAll(pageNumber, pageSize, out var rowCount, search);
        
        return Ok(new CollectionDto<AnnouncementDto>(announcements, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Announcement successfully retrieved.",
            Result = announcements
        });
    }

    [HttpGet("{announcementId:guid}")]
    public IActionResult GetAnnouncement(Guid announcementId)
    {
        var announcements = announcementService.GetById(announcementId);

        return Ok(new ResponseDto<AnnouncementDto>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Announcement successfully retrieved.",
            Result = announcements
        });

    }

    [HttpPost]
    public IActionResult InsertAnnouncement(CreateAnnouncementDto announcement)
    {
        announcementService.Create(announcement);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Announcement successfully added.",
            Result = true
        });

    }

    [HttpPut("{announcementId:guid}")]
    public IActionResult UpdateAnnouncement(UpdateAnnouncementDto announcement, Guid announcementId)
    {
        announcementService.Update(announcementId, announcement);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Announcement successfully updated.",
            Result = true
        });
    }

    [HttpPatch("{announcementId:guid}")]
    public IActionResult DeleteAnnouncement(Guid announcementId)
    {
        announcementService.Delete(announcementId);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Announcement successfully deleted.",
            Result = true
        });
    }
}