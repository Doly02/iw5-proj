// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Forms.IdentityProvider.App.Pages.Create;

public class InputModel
{
    [Required]
    public string? Username { get; set; }

    [Required]
    public string? Password { get; set; }

    [Required]
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhotoUrl { get; set; }
    

    public string? ReturnUrl { get; set; }

    public string? Button { get; set; }
}