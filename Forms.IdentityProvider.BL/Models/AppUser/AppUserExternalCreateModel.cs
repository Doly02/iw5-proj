﻿using System.Security.Claims;

namespace Forms.IdentityProvider.BL.Models;

public class AppUserExternalCreateModel
{
    public required string? Email { get; set; }
    public required string Provider { get; set; }
    public required string ProviderIdentityKey { get; set; }
    public IEnumerable<Claim> Claims { get; set; } = new List<Claim>();
}