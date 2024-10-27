using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;
using Forms.Common.Enums;

namespace Forms.API.DAL.IntegrationTests;

using System;
using System.Linq;
using Xunit;

public class QuestionSqlRepositoryTests : IClassFixture<SqlFixture>, IDisposable
{
    private readonly IQuestionRepository _questionRepository;
    private readonly SqlFixture _fixture;

    public QuestionSqlRepositoryTests()
    {
        _fixture = new SqlFixture();
        _questionRepository = _fixture.GetQuestionRepository();
    }

    [Fact]
    public void GetAll_ShouldReturnAllQuestions()
    {
        // Act
        var questions = _questionRepository.GetAll();

        // Assert
        Assert.NotNull(questions);
        Assert.Equal(_fixture.QuestionGuids.Count, questions.Count);
    }

    [Fact]
    public void GetById_ShouldReturnQuestion_WhenQuestionExists()
    {
        // Arrange
        var questionId = _fixture.QuestionGuids.First();

        // Act
        var question = _questionRepository.GetById(questionId);

        // Assert
        Assert.NotNull(question);
        Assert.Equal(questionId, question.Id);
        Assert.NotNull(question.Responses);
    }

    [Fact]
    public void GetById_ShouldReturnNull_WhenQuestionDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var question = _questionRepository.GetById(nonExistentId);

        // Assert
        Assert.Null(question); 
    }

    [Fact]
    public void Insert_ShouldAddNewQuestion()
    {
        // Arrange
        var newQuestion = new QuestionEntity
        {
            Id = Guid.NewGuid(),
            Name = "Favorite Fruit",
            Description = "Choose your favorite fruit",
            QuestionType = QuestionType.Range,
            FormId = _fixture.FormGuids.First(),
            Answer = new List<string> { "Apple", "Banana", "Orange" }
        };

        // Act
        var insertedId = _questionRepository.Insert(newQuestion);

        // Assert
        var insertedQuestion = _questionRepository.GetById(insertedId);
        Assert.NotNull(insertedQuestion);
        Assert.Equal(newQuestion.Name, insertedQuestion.Name);
        Assert.Equal(newQuestion.Description, insertedQuestion.Description);
        Assert.Equal(newQuestion.QuestionType, insertedQuestion.QuestionType);
        Assert.Equal(newQuestion.Answer, insertedQuestion.Answer);
    }

    [Fact]
    public void Update_ShouldModifyExistingQuestion()
    {
        // Arrange
        var questionId = _fixture.QuestionGuids.First();
        var existingQuestion = _questionRepository.GetById(questionId);
        if (existingQuestion == null) return;
        
        existingQuestion.Name = "Updated Question Name";
        existingQuestion.QuestionType = QuestionType.Range;
        existingQuestion.Answer = new List<string> { "1", "2", "3" };

        // Act
        var updatedId = _questionRepository.Update(existingQuestion);

        // Assert
        var updatedQuestion = _questionRepository.GetById(questionId);
        Assert.NotNull(updatedQuestion);
        Assert.Equal("Updated Question Name", updatedQuestion.Name);
        Assert.Equal(QuestionType.Range, updatedQuestion.QuestionType);
        Assert.Equal(existingQuestion.Answer, updatedQuestion.Answer);
    }

    [Fact]
    public void Update_ShouldReturnNull_WhenQuestionDoesNotExist()
    {
        // Arrange
        var nonExistentQuestion = new QuestionEntity
        {
            Id = Guid.NewGuid(),
            Name = "Non-existent question",
            Description = "Non-existent question description",
            QuestionType = QuestionType.OpenQuestion,
            FormId = Guid.NewGuid()
        };

        // Act
        var result = _questionRepository.Update(nonExistentQuestion);

        // Assert
        Assert.Null(result); 
    }

    [Fact]
    public void Remove_ShouldDeleteQuestion()
    {
        // Arrange
        var questionId = _fixture.QuestionGuids.First();
        Assert.True(_questionRepository.Exists(questionId)); 

        // Act
        _questionRepository.Remove(questionId);

        // Assert
        Assert.False(_questionRepository.Exists(questionId)); 
    }

    [Fact]
    public void Exists_ShouldReturnTrue_WhenQuestionExists()
    {
        // Arrange
        var questionId = _fixture.QuestionGuids.First();

        // Act
        var exists = _questionRepository.Exists(questionId);

        // Assert
        Assert.True(exists); 
    }

    [Fact]
    public void Exists_ShouldReturnFalse_WhenQuestionDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var exists = _questionRepository.Exists(nonExistentId);

        // Assert
        Assert.False(exists); 
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
