using System;
using Forms.Api.DAL.Common.Entities.Interfaces;

namespace CookBook.Api.DAL.Common.Entities
{
    public abstract record EntityBase : IEntity
    {
        public required Guid Id { get; init; }
    }
}