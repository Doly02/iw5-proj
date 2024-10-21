using Forms.Api.DAL.Common.Entities;

namespace Forms.Common.Models.Response;

public record ResponseDetailModel : IWithId
{
    public required Guid Id { get; init; }
    
    public required UserEntity User { get; set; }
    
    public required QuestionEntity Question { get; set; }
    
    public List<string?>? UserResponse { get; set; }
}