namespace LibraryEcom.Application.DTOs.Author;

public class CreateAuthorDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Biography { get; set; } = string.Empty;

    public DateOnly BirthDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

}