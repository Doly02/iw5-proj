namespace Forms.Common.Models.User;

public record UserListModel : IWithId
{
    public required Guid Id { get; init; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? PhotoUrl { get; set; }
}