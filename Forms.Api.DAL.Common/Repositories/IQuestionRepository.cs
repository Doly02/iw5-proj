using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;

namespace Forms.Api.DAL.Common.Repositories;

public interface IQuestionRepository : IApiRepository<QuestionEntity>, ISearchRepository<QuestionEntity>
{
    
}