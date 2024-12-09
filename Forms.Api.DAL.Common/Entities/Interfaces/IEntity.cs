using Forms.Common;

namespace Forms.Api.DAL.Common.Entities.Interfaces
{
    public interface IEntity : IWithId
    {
        string? OwnerId { get; set; }

    }
}