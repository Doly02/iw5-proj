using System.ComponentModel.DataAnnotations;
using Forms.Api.DAL.Common.Entities;
using Forms.Common.Models.Question;
using Forms.Common.Models.User;

namespace Forms.Common.Models.Form;

public record FormDetailModel : IWithId
{
    public required Guid Id { get; init; }
    
    [Required(ErrorMessage = "Name is required")]
    public required string Name { get; set; }
    
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Opening date is required")]
    public required DateTime DateOpen { get; set; }
    
    [Required(ErrorMessage = "Closing date is required")]
    public required DateTime DateClose { get; set; }
    
    
    public UserListModel User { get; set; }
    public required Guid UserId { get; set; }
    
    public IList<QuestionDetailModel> Questions { get; set; } = new List<QuestionDetailModel>();
    
    
}