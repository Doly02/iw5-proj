using System;
using System.Collections.Generic;
using Forms.Common.BL.Facades;
using Forms.Common.Models.Response;

namespace Forms.Api.BL.Facades
{
    public interface IResponseFacade : IAppFacade
    {
        List<ResponseListModel> GetAll();

        ResponseDetailModel? GetById(Guid id);
        
        Guid CreateOrUpdate(ResponseDetailModel questionModel);
        
        Guid Create(ResponseDetailModel questionModel);
        
        Guid? Update(ResponseDetailModel questionModel);
        
        void Delete(Guid id);
        public List<ResponseDetailModel> GetByQuestionId(Guid questionId);
    }
}