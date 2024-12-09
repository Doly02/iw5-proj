using AutoMapper;

namespace Forms.Api.DAL.Common.Entities
{
    public record UserEntity : EntityBase
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string? PhotoUrl { get; set; }

        public ICollection<ResponseEntity> Responses { get; set; } = new List<ResponseEntity>();
        public ICollection<FormEntity> Forms { get; set; } = new List<FormEntity>();
    }
    
    public class UserEntityMapperProfile : Profile
    {
        public UserEntityMapperProfile()
        {
            CreateMap<UserEntity, UserEntity>();
        }
    }
}