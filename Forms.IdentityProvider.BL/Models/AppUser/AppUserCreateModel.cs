﻿namespace Forms.IdentityProvider.BL.Models;

public class AppUserCreateModel
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string Subject { get; set; }
    public required string Email { get; set; }
}
