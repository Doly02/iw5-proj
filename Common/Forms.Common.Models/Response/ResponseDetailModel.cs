using System;
using System.ComponentModel.DataAnnotations;
using Forms.Common.Models.Resources.Texts;

namespace Forms.Common.Models.Response

{
    public record ResponseDetailModel : IWithId
    {
        public required Guid Id { get; init; }

        [Required(ErrorMessageResourceName = nameof(ResponseDetailModelResources.Response_Required_ErrorMessage), ErrorMessageResourceType = typeof(ResponseDetailModelResources))]
        public required string UserResponse { get; set; }
        
    }
}