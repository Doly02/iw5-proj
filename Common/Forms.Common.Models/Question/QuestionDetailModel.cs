using System;
using System.ComponentModel.DataAnnotations;
using Forms.Common.Models.Resources.Texts;

namespace Forms.Common.Models.Question

{
    public record QuestionDetailModel : IWithId
    {
        public required Guid Id { get; init; }

        [Required(ErrorMessageResourceName = nameof(QuestionDetailModelResources.Name_Required_ErrorMessage), ErrorMessageResourceType = typeof(QuestionDetailModelResources))]
        public required string Name { get; set; }
        
        [Required(ErrorMessageResourceName = nameof(QuestionDetailModelResources.Description_Required_ErrorMessage), ErrorMessageResourceType = typeof(QuestionDetailModelResources))]
        public required string Answer { get; set; }
        
    }
}