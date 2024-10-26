using System;
using System.Collections.Generic;
using Forms.Common.BL.Facades;
using Forms.Common.Models.Question;

namespace Forms.Api.BL.Facades
{
    public interface IQuestionFacade : IAppFacade, ISearchFacade
    {
        List<QuestionListModel> GetAll();
        QuestionDetailModel? GetById(Guid id);
        Guid CreateOrUpdate(QuestionDetailModel questionModel);
        Guid Create(QuestionDetailModel questionModel);
        Guid? Update(QuestionDetailModel questionModel);
        void Delete(Guid id);
    }
}