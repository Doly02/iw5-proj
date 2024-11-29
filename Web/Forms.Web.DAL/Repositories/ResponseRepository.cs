
using Forms.Common.Models.Response;

namespace Forms.Web.DAL.Repositories
{
    public class ResponseRepository : RepositoryBase<ResponseDetailModel>
    {
        public override string TableName { get; } = "responses";

        public ResponseRepository(LocalDb localDb)
            : base(localDb)
        {
        }
    }
}