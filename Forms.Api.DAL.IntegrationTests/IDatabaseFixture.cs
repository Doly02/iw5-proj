using System;
using System.Collections.Generic;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;

namespace Forms.API.DAL.IntegrationTests;

public interface IDatabaseFixture
{
    QuestionEntity? GetQuestionDirectly(Guid questionId);
    UserEntity? GetUserDirectly(Guid userId);
    FormEntity? GetFormDirectly(Guid formId);
    IUserRepository GetUserRepository();
    IFormRepository GetFormRepository();
    IQuestionRepository GetQuestionRepository();
    IList<Guid> UserGuids { get; }
    IList<Guid> QuestionGuids { get; }
    IList<Guid> FormGuids { get; }
}