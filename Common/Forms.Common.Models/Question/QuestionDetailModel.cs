using System;
using System.ComponentModel.DataAnnotations;
using Forms.Common.Enums;
using Forms.Common.Models.Resources.Texts;
using Forms.Common.Models.Response;

namespace Forms.Common.Models.Question

{
    public record QuestionDetailModel : IWithId
    {
        public required Guid Id { get; init; }

        [Required(ErrorMessageResourceName = nameof(QuestionDetailModelResources.Name_Required_ErrorMessage), ErrorMessageResourceType = typeof(QuestionDetailModelResources))]
        public required string Name { get; set; }
        
        [Required(ErrorMessageResourceName = nameof(QuestionDetailModelResources.Description_Required_ErrorMessage), ErrorMessageResourceType = typeof(QuestionDetailModelResources))]
        public required string Description { get; set; }
        
        public List<string>? Answer { get; set; } 
        
        public required QuestionType QuestionType { get; set; }
        
        public IList<ResponseDetailModel> Responses { get; set; } = new List<ResponseDetailModel>();
        
    }
}