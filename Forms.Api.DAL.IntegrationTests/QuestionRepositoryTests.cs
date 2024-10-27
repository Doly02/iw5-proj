using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Common.Enums;
using Forms.Api.DAL.Memory;
using Xunit;

namespace Forms.API.DAL.IntegrationTests;

public class QuestionRepositoryTests
{
    private readonly IDatabaseFixture _dbFixture;
    public QuestionRepositoryTests()
    {
        _dbFixture = new InMemoryDatabaseFixture();
    }
    
    [Fact]
    public void GetById_Returns_Question_With_Responses()
    {
        /* Arrange */
        var questionRepository = _dbFixture.GetQuestionRepository();
        var questionId = _dbFixture.QuestionGuids[0];

        /* Act */
        var question = questionRepository.GetById(questionId);

        /* Assert */
        Assert.NotNull(question);
        Assert.Equal(questionId, question.Id);
        Assert.NotEmpty(question.Responses);

        var response1 = question.Responses.SingleOrDefault(r => r.Id == _dbFixture.ResponseGuids[0]);
        Assert.NotNull(response1);
        Assert.Equal("Odpoved od Johna na Otazku 1", response1.UserResponse!.First());

        var response2 = question.Responses.SingleOrDefault(r => r.Id == _dbFixture.ResponseGuids[1]);
        Assert.NotNull(response2);
        Assert.Equal("Odpoved od Jane na Otazku 1", response2.UserResponse!.First());
    }
    
    [Fact]
    public void Insert_Adds_New_Question()
    {
        /* Arrange */
        var questionRepository = _dbFixture.GetQuestionRepository();
        var newQuestionId = Guid.NewGuid();

        var newQuestion = new QuestionEntity
        {
            Id = newQuestionId,
            Name = "Nová otázka",
            Description = "Popis otázky",
            QuestionType = QuestionType.OpenQuestion,
            FormId = _dbFixture.FormGuids[0]
        };

        /* Act */
        questionRepository.Insert(newQuestion);

        /* Assert */
        var questionFromDb = questionRepository.GetById(newQuestionId);
        Assert.NotNull(questionFromDb);
        Assert.Equal(newQuestionId, questionFromDb.Id);
        Assert.Equal("Nová otázka", questionFromDb.Name);
    }

    [Fact]
    public void Update_Updates_Question_Details()
    {
        /* Arrange */
        var questionRepository = _dbFixture.GetQuestionRepository();
        var questionId = _dbFixture.QuestionGuids[0];
        var question = questionRepository.GetById(questionId);

        var updatedName = "Aktualizovaná otázka";
        if (question != null)
        {
            question.Name = updatedName;

            /* Act */
            questionRepository.Update(question);
        }

        /* Assert */
        var questionFromDb = questionRepository.GetById(questionId);
        Assert.NotNull(questionFromDb);
        Assert.Equal(updatedName, questionFromDb.Name);
        Assert.NotEmpty(questionFromDb.Responses);
    }
    
}