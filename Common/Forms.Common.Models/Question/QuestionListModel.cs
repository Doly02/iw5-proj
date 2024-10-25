using Forms.Api.DAL.Common.Entities;
using Forms.Common.Enums;

namespace Forms.Common.Models.Question;

public class QuestionListModel
{
    public required string Name { get; set; }
        
    public required string Description { get; set; }
    
    public required QuestionType QuestionType { get; set; }
    
    public List<string>? Answer { get; set; } 
}