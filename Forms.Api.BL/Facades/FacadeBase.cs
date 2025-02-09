﻿using Forms.Api.DAL.Common.Entities.Interfaces;
using Forms.Api.DAL.Common.Repositories;
using System;

namespace Forms.Api.BL.Facades;

public class FacadeBase<TRepository, TEntity>
    where TRepository : IApiRepository<TEntity>
    where TEntity : IEntity
{
    private readonly TRepository repository;

    public FacadeBase(TRepository repository)
    {
        this.repository = repository;
    }

    protected void ThrowIfWrongOwner(Guid id, string? ownerId)
    {
        var IdOwner = repository.GetById(id)?.OwnerId;
        if (ownerId is not null
            && IdOwner != ownerId)
        {
            throw new UnauthorizedAccessException();
        }
    }
    

}

