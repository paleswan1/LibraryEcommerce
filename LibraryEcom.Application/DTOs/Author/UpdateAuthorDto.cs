namespace LibraryEcom.Application.DTOs.Author;

public class UpdateAuthorDto
{
    public string Name { get; set; } = string.Empty;

    public string Biography { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; } = DateTime.UtcNow;
}