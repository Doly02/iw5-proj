using System.ComponentModel.DataAnnotations;

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
    
    [Required(ErrorMessage = "Password is required")]
    public required string PasswordHash { get; set; }

    public string? PhotoUrl { get; set; }


    // todo
    // public IList<ResponseModel> Responses { get; set; } = new List<ResponseModel>();
    // public IList<FormModel> Forms { get; set; } = new List<FormModel>();
}