using Forms.Api.DAL.Common.Entities;

namespace Forms.Common.Models.Response;

public record ResponseListModel : IWithId
{
    public required Guid Id { get; init; }

    public required Guid UserId { get; set; }

    public required Guid QuestionId { get; set; }
    
    public List<string>? UserResponse { get; set; }
}