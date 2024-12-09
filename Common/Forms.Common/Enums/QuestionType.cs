using System.Resources;
using Forms.Common.Attributes;
using Forms.Common.Resources;

namespace Forms.Common.Enums
{
    public enum QuestionType
    {
        [QuestionTypeDescription(nameof(QuestionTypeResources.OpenQuestionDescription))]
        OpenQuestion = 0,

        [QuestionTypeDescription(nameof(QuestionTypeResources.SelectionFromOptionsDescription))]
        Selection = 1,
        
        [QuestionTypeDescription(nameof(QuestionTypeResources.RangeDescription))]
        Range = 2,
    }


    public class QuestionTypeDescription : LocalizableDescriptionAttribute
    {
        public QuestionTypeDescription(string resourceName) : base(resourceName)
        {
        }

        protected override ResourceManager GetResource()
        {
            return QuestionTypeResources.ResourceManager;
        }
    }
}