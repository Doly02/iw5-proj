using Forms.Api.DAL.Common.Entities;

namespace Forms.Common.Models.Response;

public record ResponseListModel : IWithId
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; set; }  // todo treba?
    public required Guid QuestionId { get; set; }  // todo treba?
    public required UserEntity User { get; set; }
    public required QuestionEntity Question { get; set; }
    public List<string?>? UserResponse { get; set; }
}