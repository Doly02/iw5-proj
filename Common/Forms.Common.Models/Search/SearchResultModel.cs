namespace Forms.Common.Models.Search;

public record SearchResultModel : IWithId
{
    public Guid Id { get; init; }
    public string Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}