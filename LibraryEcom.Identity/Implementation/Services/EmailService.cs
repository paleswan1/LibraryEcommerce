using System.Net;
using System.Net.Mail;
using LibraryEcom.Application.DTOs.Email;
using LibraryEcom.Application.Exceptions;
using LibraryEcom.Application.Interfaces.Services;
using LibraryEcom.Application.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using LibraryEcom.Domain.Common;
using LibraryEcom.Domain.Common.Property;

namespace LibraryEcom.Infrastructure.Implementation.Services;

public class EmailService(
    IOptions<MailSettings> smtpSettings,
    IWebHostEnvironment webHostEnvironment) : IEmailService
{
    private readonly MailSettings _mailSettings = smtpSettings.Value;

    private const string EmailPath = Constants.FilePath.EmailTemplateFilePath;
    
    public async Task SendEmail(EmailDto emailDto)
    {
        try
        {
            var fromAddress = new MailAddress(_mailSettings.Username, "LibraryEcom");
            var toAddress = new MailAddress(emailDto.ToEmailAddress, emailDto.FullName);

            var message = new MailMessage
            {
                From = fromAddress,
                Subject = emailDto.Subject,
                IsBodyHtml = true
            };

            message.To.Add(toAddress);
            message.Body = PrepareEmailBody(emailDto);

            if (!string.IsNullOrEmpty(emailDto.Cc))
            {
                message.CC.Add(emailDto.Cc);
            }

            using var smtpClient = new SmtpClient(_mailSettings.Host, _mailSettings.Port)
            {
                Credentials = new NetworkCredential(_mailSettings.Username, _mailSettings.Password),
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            throw new BadRequestException("Failed to send email.", [ex.Message]);
        }
    }

    
    private string PrepareEmailBody(EmailDto dto)
    {
        dto.PlaceHolders = GetPlaceholders(dto);

        var templateName = dto.Subject.Replace(" ", "");
        var htmlTemplate = GetTemplate(templateName);

        foreach (var placeholder in dto.PlaceHolders)
        {
            htmlTemplate = htmlTemplate.Replace(placeholder.Key, placeholder.Value);
        }

        return htmlTemplate;
    }

    private string GetTemplate(string templateName)
    {
        var templatePath = Path.Combine(webHostEnvironment.WebRootPath, EmailPath, $"{templateName}.html");
        return File.Exists(templatePath)
            ? File.ReadAllText(templatePath)
            : throw new FileNotFoundException("Email template not found", templatePath);
    }

    private static List<KeyValuePair<string, string>> GetPlaceholders(EmailDto dto)
    {
        return new()
        {
            new("{$fullName}", dto.FullName),
            new("{$userName}", dto.UserName ?? ""),
            new("{$password}", dto.Password ?? ""),
            new("{$primaryMessage}", dto.PrimaryMessage ?? ""),
            new("{$secondaryMessage}", dto.SecondaryMessage ?? ""),
            new("{$tertiaryMessage}", dto.TertiaryMessage ?? ""),
            new("{$remarks}", dto.Remarks ?? ""),
            new("{$role}", dto.Role ?? "")
        };
    }

    
}
