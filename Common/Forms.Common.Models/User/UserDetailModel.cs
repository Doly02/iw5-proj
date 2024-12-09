using System.ComponentModel.DataAnnotations;
using Forms.Common.Models.Form;

namespace Forms.Common.Models.User;

public record UserDetailModel : IWithId
{
    public required Guid Id { get; init; }

    [Required(ErrorMessage = "First name is required")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    public required string LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; set; }

    public string? PhotoUrl { get; set; }
    
    public IList<FormDetailModel> Forms { get; set; } = new List<FormDetailModel>();
}