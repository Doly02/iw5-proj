using System;
using AutoMapper;
using Forms.Common.Enums;

namespace Forms.Api.DAL.Common.Entities
{
    public record QuestionEntity : EntityBase
    {
        public required string Name { get; set; }
        
        public required string Description { get; set; }
        
        public required QuestionType QuestionType { get; set; }
        
        public Guid FormId { get; set; }
        public required FormEntity Form { get; set; }
        public ICollection<ResponseEntity> Responses { get; set; } = new List<ResponseEntity>();
        
        public List<string> Answer { get; set; } = new List<string>();
    }

    public class QuestionEntityMapperProfile : Profile
    {
        public QuestionEntityMapperProfile()
        {
            CreateMap<QuestionEntity, QuestionEntity>();
        }
    }
}