namespace LibraryEcom.Application.Exceptions;

public class BadRequestException(string message, string[] validationErrors) : Exception(message)
{
    public string[] ValidationErrors { get; set; } = validationErrors;
}