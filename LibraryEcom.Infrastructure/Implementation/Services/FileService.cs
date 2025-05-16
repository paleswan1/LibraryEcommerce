using LibraryEcom.Application.Interfaces.Services;
using LibraryEcom.Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace LibraryEcom.Infrastructure.Implementation.Services;

public class FileService(IWebHostEnvironment webHostEnvironment):IFileService
{
    public string UploadDocument(IFormFile file, string uploadedFilePath, string? prefix = null)
    {
        if (!Directory.Exists(Path.Combine(webHostEnvironment.WebRootPath, uploadedFilePath)))
        {
            Directory.CreateDirectory(Path.Combine(webHostEnvironment.WebRootPath, uploadedFilePath));
        }

        var uploadedDocumentPath = Path.Combine(webHostEnvironment.WebRootPath, uploadedFilePath);

        var fileName = UploadFile(file, uploadedDocumentPath);

        return fileName;
    }

    public string UploadDocument(string base64Image, string uploadedFilePath, string? prefix = null)
    {
        return string.Empty;
    }
    
    public void DeleteFile(string uploadedFilePath)
    {
        var fullPath = Path.Combine(webHostEnvironment.WebRootPath, uploadedFilePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    public void DeleteFolder(string folderPath)
    {
        var fullPath = Path.Combine(webHostEnvironment.WebRootPath, folderPath);

        if (Directory.Exists(fullPath))
        {
            Directory.Delete(fullPath, recursive: true);
        }
    }
    
    public string FileExistPath(string uploadedFilePath)
    {
        var fullPath = Path.Combine(webHostEnvironment.WebRootPath, uploadedFilePath);

        return File.Exists(fullPath) ? fullPath : "";
    }
    
    private static string UploadFile(IFormFile file, string uploadedFilePath, string? prefix = null)
    {
        var extension = Path.GetExtension(file.FileName);

        var fileName = string.IsNullOrEmpty(prefix) ? extension.SetUniqueFileName() : $"{prefix} - {extension.SetUniqueFileName()}";

        using var stream = new FileStream(Path.Combine(uploadedFilePath, fileName), FileMode.Create);

        file.CopyTo(stream);

        return fileName;
    }
}