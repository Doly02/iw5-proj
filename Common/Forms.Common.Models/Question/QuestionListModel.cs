using Forms.Api.DAL.Common.Entities;
using Forms.Common.Enums;

namespace Forms.Common.Models.Question;

public record QuestionListModel : IWithId
{
    public required Guid Id { get; init; }

    public required string Name { get; set; }
        
    public required string Description { get; set; }
    
    public required QuestionType QuestionType { get; set; }
    
    public List<string>? Answer { get; set; } 
}