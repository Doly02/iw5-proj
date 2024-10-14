using AutoMapper;

namespace Forms.Api.DAL.Common.Entities
{
    public record ResponseEntity : EntityBase
    {
        public Guid UserId { get; set; }
        public Guid QuestionId { get; set; }
        
        public required UserEntity User { get; set; }
        public required QuestionEntity Question { get; set; }
        
        public string? UserResponse { get; set; }
    }
    
    public class ResponseEntityMapperProfile : Profile
    {
        public ResponseEntityMapperProfile()
        {
            CreateMap<ResponseEntity, ResponseEntity>();
        }
    }
}


