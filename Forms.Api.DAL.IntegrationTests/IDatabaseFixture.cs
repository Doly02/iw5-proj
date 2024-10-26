using System;
using System.Collections.Generic;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;

namespace Forms.API.DAL.IntegrationTests;

public interface IDatabaseFixture
{
    QuestionEntity? GetQuestionDirect(Guid userId);
    
    UserEntity? GetUserDirectly(Guid userId);
    FormEntity? GetFormDirectly(Guid formId);
    IUserRepository GetUserRepository();
    IList<Guid> UserGuids { get; }
    IList<Guid> QuestionGuids { get; }
    IList<Guid> FormGuids { get; }
}