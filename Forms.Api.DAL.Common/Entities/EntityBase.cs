using System;
using Forms.Api.DAL.Common.Entities.Interfaces;

namespace Forms.Api.DAL.Common.Entities
{
    public abstract record EntityBase : IEntity
    {
        public required Guid Id { get; init; }
    }
}