using Forms.Common.Models;
using Forms.Common.Models.Question;

namespace Forms.Web.DAL.Repositories
{
    public class QuestionRepository : RepositoryBase<QuestionDetailModel>
    {
        public override string TableName { get; } = "questions";

        public QuestionRepository(LocalDb localDb)
            : base(localDb)
        {
        }
    }
}