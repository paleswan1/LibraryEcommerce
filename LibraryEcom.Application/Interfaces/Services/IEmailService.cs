using LibraryEcom.Application.Common.Service;
using LibraryEcom.Application.DTOs.Email;

namespace LibraryEcom.Application.Interfaces.Services;

public interface IEmailService: ITransientService
{
    Task SendEmail(EmailDto emailDto);

}