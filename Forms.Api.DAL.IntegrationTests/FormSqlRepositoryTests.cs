using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;
using Forms.Common.Enums;

namespace Forms.API.DAL.IntegrationTests;

using System;
using System.Linq;
using Xunit;

public class FormSqlRepositoryTests : IClassFixture<SqlFixture>, IDisposable
{
    private readonly IFormRepository _formRepository;
    private readonly SqlFixture _fixture;

    public FormSqlRepositoryTests()
    {
        _fixture = new SqlFixture();
        _formRepository = _fixture.GetFormRepository();
    }

    [Fact]
    public void GetAll_ShouldReturnAllForms()
    {
        /* Act */ 
        var forms = _formRepository.GetAll();

        /* Assert */
        Assert.NotNull(forms);
        Assert.Equal(_fixture.FormGuids.Count, forms.Count);
    }
    
    [Fact]
    public void GetById_ShouldReturnForm_WhenFormExists()
    {
        /* Arrange */
        var formId = _fixture.FormGuids.First();

        /* Act */
        var form = _formRepository.GetById(formId);

        /* Assert */
        Assert.NotNull(form);
        Assert.Equal(formId, form.Id); 
    }
    
    [Fact]
    public void Exists_ShouldReturnFalse_WhenFormDoesNotExist()
    {
        /* Arrange */
        var nonExistentId = Guid.NewGuid();

        /* Act */
        var exists = _formRepository.Exists(nonExistentId);

        /* Assert */
        Assert.False(exists); 
    }
    
    [Fact]
    public void Remove_ShouldDeleteForm()
    {
        /* Arrange */
        var formId = _fixture.FormGuids.First();
        Assert.True(_formRepository.Exists(formId)); 

        /* Act */
        _formRepository.Remove(formId);

        /* Assert */
        Assert.False(_formRepository.Exists(formId)); 
    }
    
    [Fact]
    public void Update_ShouldModifyExistingForm()
    {
        /* Arrange */
        var formId = _fixture.FormGuids.First();
        var existingForm = _formRepository.GetById(formId);
        existingForm.Description = "New Question Description!";

        /* Act */
        var updatedId = _formRepository.Update(existingForm);

        /* Assert */
        var updatedForm = _formRepository.GetById(formId);
        Assert.NotNull(updatedForm);
        Assert.Equal("New Question Description!", updatedForm.Description);
    }
    
    [Fact]
    public void CreateQuestion_ShouldAddQuestionToForm()
    {
        var questionRepository = _fixture.GetQuestionRepository();
        
        /* Arrange */
        var formId = _fixture.FormGuids[0];
        var newQuestion = new QuestionEntity
        {
            Id = Guid.NewGuid(),
            Name = "New Question",
            Description = "Question Description",
            QuestionType = QuestionType.OpenQuestion,
            FormId = formId
        };

        /* Act */
        questionRepository.Insert(newQuestion);

        /* Assert */
        var form = _formRepository.GetById(formId);
        Assert.NotNull(form);
        Assert.Contains(form.Questions, q => q.Id == newQuestion.Id);
    }
    
    [Fact]
    public void Update_ShouldReturnNull_WhenFormDoesNotExist()
    {
        /* Arrange */
        var userId = _fixture.UserGuids[0];
        var nonExistentForm = new FormEntity
        {
            Id = Guid.NewGuid(),
            Name = "VUT FIT",
            Description = "No FIT",
            DateOpen = DateTime.Now.AddDays(-5),
            DateClose = DateTime.Now.AddDays(2),
            UserId = userId,
            Questions = new List<QuestionEntity>() 
        };
        
        /* Act */
        var result = _formRepository.Update(nonExistentForm);

        /* Assert */
        Assert.Null(result); 
    }
    
    public void Dispose()
    {
        _fixture.Dispose();
    }
}
