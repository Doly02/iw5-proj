using System;
using AutoMapper;

namespace Forms.Api.DAL.Common.Entities
{
    public record QuestionEntity : EntityBase
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Answer { get; set; }
    }

    public class IngredientEntityMapperProfile : Profile
    {
        public IngredientEntityMapperProfile()
        {
            CreateMap<QuestionEntity, QuestionEntity>();
        }
    }
}