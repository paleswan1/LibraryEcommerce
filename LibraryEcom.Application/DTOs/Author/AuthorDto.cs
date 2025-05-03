namespace LibraryEcom.Application.DTOs.Author;

public class AuthorDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Biography { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }
}