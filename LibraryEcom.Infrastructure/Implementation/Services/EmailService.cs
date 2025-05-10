using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using LibraryEcom.Domain.Common;
using LibraryEcom.Domain.Common.Enum;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using LibraryEcom.Application.Settings;
using LibraryEcom.Application.DTOs.Email;
using LibraryEcom.Application.Exceptions;
using LibraryEcom.Application.Interfaces.Services;
using LibraryEcom.Domain.Common.Property;

namespace LibraryEcom.Infrastructure.Implementation.Services;

public class EmailService(IOptions<MailSettings> smtpSettings, IWebHostEnvironment webHostEnvironment) : IEmailService
{
    private readonly MailSettings _mailSettings = smtpSettings.Value;
    private const string EmailPath = Constants.FilePath.EmailTemplateFilePath;

    public async Task SendEmail(EmailDto emailDto)
    {
        try
        {
            using var emailMessage = new MimeMessage();

            var emailFrom = new MailboxAddress("LibraryEcom", _mailSettings.Username);
            var emailTo = new MailboxAddress(emailDto.FullName, emailDto.ToEmailAddress);
            var emailBcc = new MailboxAddress("LibraryEcom", _mailSettings.Username);

            emailMessage.From.Add(emailFrom);
            emailMessage.To.Add(emailTo);
            emailMessage.Bcc.Add(emailBcc);

            if (!string.IsNullOrEmpty(emailDto.Cc))
            {
                var emailCc = new MailboxAddress(emailDto.Cc, emailDto.Cc);
                emailMessage.Cc.Add(emailCc);
            }

            emailMessage.Subject = emailDto.Subject;

            emailDto.PlaceHolders = GetPlaceHolders(emailDto);
            emailDto.Body = PrepareTemplate(emailDto);

            var emailBodyBuilder = new BodyBuilder()
            {
                HtmlBody = emailDto.Body
            };

            emailMessage.Body = emailBodyBuilder.ToMessageBody();

            using var mailClient = new SmtpClient();
            await mailClient.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            await mailClient.AuthenticateAsync(_mailSettings.Username, _mailSettings.Password);
            await mailClient.SendAsync(emailMessage);
            await mailClient.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            throw new BadRequestException("Email could not be sent.", [ex.Message]);
        }
    }

    private static List<KeyValuePair<string, string>> GetPlaceHolders(EmailDto emailDto)
    {
        var result = new List<KeyValuePair<string, string>>();

        switch (emailDto.EmailProcess)
        {
            case EmailProcess.SelfRegistration:
                result.Add(new KeyValuePair<string, string>("{$fullName}", emailDto.FullName));
                result.Add(new KeyValuePair<string, string>("{$userName}", emailDto.UserName ?? ""));
                result.Add(new KeyValuePair<string, string>("{$primaryMessage}", emailDto.PrimaryMessage ?? ""));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return result;
    }

    private static string UpdatePlaceHolders(string text, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
    {
        if (string.IsNullOrEmpty(text)) return text;

        foreach (var placeholder in keyValuePairs.Where(placeholder => text.Contains(placeholder.Key)))
        {
            text = text.Replace(placeholder.Key, placeholder.Value);
        }

        return text;
    }

    private string PrepareTemplate(EmailDto emailDto)
    {
        return UpdatePlaceHolders(GetEmailBody(emailDto.EmailProcess.ToString()), emailDto.PlaceHolders);
    }

    private string GetEmailBody(string templateName)
    {
        return File.ReadAllText(Path.Combine(webHostEnvironment.WebRootPath, EmailPath, $"{templateName}.html"));
    }
}