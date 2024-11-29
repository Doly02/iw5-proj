using Forms.Common.Models.User;

namespace Forms.Web.DAL.Repositories;

public class UserRepository : RepositoryBase<UserDetailModel>
{
    public override string TableName { get; } = "users";

    public UserRepository(LocalDb localDb)
        : base(localDb)
    {
    }
}