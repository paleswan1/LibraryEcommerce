using LibraryEcom.Application.Common.Service;
using Microsoft.AspNetCore.Http;

namespace LibraryEcom.Application.Interfaces.Services;

public interface IFileService: ITransientService
{
    string UploadDocument(IFormFile file, string uploadedFilePath, string? prefix = null);

    string UploadDocument(string base64Image, string uploadedFilePath, string? prefix = null);

    void DeleteFile(string uploadedFilePath);

    void DeleteFolder(string folderPath);
    
    string FileExistPath(string uploadedFilePath);
}