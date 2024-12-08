using Forms.Common.Models.Form;

namespace Forms.Web.DAL.Repositories;

public class FormRepository : RepositoryBase<FormDetailModel>
{
    public override string TableName { get; } = "forms";

    public FormRepository(LocalDb localDb)
        : base(localDb)
    {
    }
}