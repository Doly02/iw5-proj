using System;

namespace Forms.Common
{
    public interface IWithId
    {
        Guid Id { get; init; }
    }
}