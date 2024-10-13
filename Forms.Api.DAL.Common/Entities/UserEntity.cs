namespace Forms.Api.DAL.Common.Entities
{
    public record UserEntity
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; } 
        public string? PhotoUrl { get; set; }
    }
}