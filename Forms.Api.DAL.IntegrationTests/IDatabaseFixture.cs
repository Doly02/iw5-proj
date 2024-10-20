using System;
using System.Collections.Generic;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;

namespace Forms.API.DAL.IntegrationTests;

public interface IDatabaseFixture
{
    QuestionEntity? GetQuestionDirect(Guid userId);
    
    IList<Guid> QuestionGuids { get; }
}