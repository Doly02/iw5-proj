namespace Forms.Common.Models.Form;

public record FormListModel : IWithId
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required DateTime DateOpen { get; set; }
    public required DateTime DateClose { get; set; }
    public required Guid UserId { get; set; }
}