using AutoMapper;

namespace Forms.Api.DAL.Common.Entities
{
    public record FormEntity : EntityBase
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required DateTime DateOpen { get; set; }
        public required DateTime DateClose { get; set; }

        public ICollection<QuestionEntity> Questions { get; set; } = new List<QuestionEntity>();
    }

    public class FormEntityMapperProfile : Profile
    {
        public FormEntityMapperProfile()
        {
            CreateMap<FormEntity, FormEntity>();
        }
    }
}