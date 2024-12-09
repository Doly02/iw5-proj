using Forms.Api.DAL.Common.Entities;
using Forms.Common.Models.Question;
using Forms.Common.Models.User;

namespace Forms.Common.Models.Response;

public record ResponseDetailModel : IWithId
{
    public required Guid Id { get; init; }
    
    public required UserDetailModel User { get; set; }
    
    public required QuestionListModel Question { get; set; }
    
    public List<string>? UserResponse { get; set; }
}