using Forms.Api.DAL.Common.Entities;
using Forms.Common.Enums;
using Forms.Api.DAL.Memory;
using Xunit;

namespace Forms.API.DAL.IntegrationTests;

public class FormRepositoryTests
{
    private readonly IDatabaseFixture _dbFixture;

    public FormRepositoryTests()
    {
        _dbFixture = new InMemoryDatabaseFixture();
    }
    
    [Fact]
    public void GetById_Returns_Requested_Form_Including_Specific_Questions()
    {
        /* Arrange */ 
        var formRepository = _dbFixture.GetFormRepository();

        /* Act */
        var form = formRepository.GetById(_dbFixture.FormGuids[0]);

        /* Assert */
        Assert.NotNull(form);
        Assert.Equal(_dbFixture.FormGuids[0], form.Id);
        
        /* Check If Form Has At Least One Question */
        Assert.NotEmpty(form.Questions);

        /* Find Specific Questions */
        var question1 = form.Questions.SingleOrDefault(q => q.Id == _dbFixture.QuestionGuids[0]);
        var question2 = form.Questions.SingleOrDefault(q => q.Id == _dbFixture.QuestionGuids[1]);

        /* Check If Questions Exist */
        Assert.NotNull(question1);
        Assert.NotNull(question2);

        /* Check Attributes of Each Question */
        Assert.Equal("Otazka cislo jedna", question1!.Name);
        Assert.Equal("Napis", question1.Description);
        Assert.Equal(QuestionType.OpenQuestion, question1.QuestionType);

        Assert.Equal("Otazka cislo dva", question2!.Name);
        Assert.Equal("Napis 2", question2.Description);
        Assert.Equal(QuestionType.Selection, question2.QuestionType);
    }
    
    [Fact]
    public void Update_Saves_NewQuestion()
    {
        /* Arrange */
        var formRepository = _dbFixture.GetFormRepository();

        var formId = _dbFixture.FormGuids[0];
        var form = _dbFixture.GetFormDirectly(formId);
        Assert.Equal(_dbFixture.FormGuids[0], form.Id);
        Assert.Equal("VUT FIT", form.Name);

        var newQuestionId = Guid.NewGuid();
        var newQuestion = new QuestionEntity
        {
            Id = newQuestionId,
            Name = "Kam byste chtěli jet na dovolenou?",
            Description = "Zadejte svůj oblíbený dovolenkový cíl.",
            QuestionType = QuestionType.OpenQuestion,
            FormId = formId
        };

        /* Act */
        form.Questions.Add(newQuestion);
        formRepository.Update(form);

        /* Assert */
        var formFromDb = _dbFixture.GetFormDirectly(formId);
        Assert.NotNull(formFromDb);
        Assert.Single(formFromDb.Questions, q => q.Id == newQuestionId);

        var questionFromDb = formFromDb.Questions.SingleOrDefault(q => q.Id == newQuestionId);
        Assert.NotNull(questionFromDb);
        Assert.Equal(newQuestion.Name, questionFromDb.Name);
        Assert.Equal(newQuestion.Description, questionFromDb.Description);
        Assert.Equal(newQuestion.QuestionType, questionFromDb.QuestionType);
    }
    
    [Fact]
    public void RemoveQuestion_Removes_Question_From_Form()
    {
        /* Arrange */
        var formRepository = _dbFixture.GetFormRepository();

        var formId = _dbFixture.FormGuids[0];
        var form = _dbFixture.GetFormDirectly(formId);
        Assert.Equal(_dbFixture.FormGuids[0], form.Id);
        Assert.Equal("VUT FIT", form.Name);

        var questionId = Guid.NewGuid();

        /* Add New Question To The Form */
        var newQuestion = new QuestionEntity
        {
            Id = questionId,
            Name = "Kam byste chtěli jet na dovolenou?",
            Description = "Zadejte svůj oblíbený dovolenkový cíl.",
            QuestionType = QuestionType.OpenQuestion,
            FormId = formId
        };

        form.Questions.Add(newQuestion);
        formRepository.Update(form);

        /* Assert: Check if Question Was Added */
        var formFromDb = _dbFixture.GetFormDirectly(formId);
        
        /* Act: Remove Question From Form */
        form.Questions.Remove(newQuestion);
        formRepository.Update(form);

        /* Assert: Check If Question Was Removed */
        formFromDb = _dbFixture.GetFormDirectly(formId);
        Assert.NotNull(formFromDb);
        Assert.DoesNotContain(formFromDb.Questions, q => q.Id == questionId);

        /* Verify That The Question Itself Is Not Found In The Form */
        var questionFromDb = formFromDb.Questions.SingleOrDefault(q => q.Id == questionId);
        Assert.Null(questionFromDb);
    }
    
    [Fact]
    public void UpdateQuestion_Changes_Question_Name_For_Form()
    {
        /* Arrange */
        var formRepository = _dbFixture.GetFormRepository();
        var formId = _dbFixture.FormGuids[0];
        var form = _dbFixture.GetFormDirectly(formId);
        /* Store First Question Of The Form */ 
        var questionToUpdate = form.Questions.First();
        var originalQuestionId = questionToUpdate.Id;

        /* Update Question's Name And Store Changes */
        var newQuestionName = "Updated Question Name";
        questionToUpdate.Name = newQuestionName;

        /* Act */
        formRepository.Update(form);

        /* Assert */
        var formFromDb = _dbFixture.GetFormDirectly(formId);
        Assert.NotNull(formFromDb);
        var updatedQuestion = formFromDb.Questions.Single(q => q.Id == originalQuestionId);
        Assert.Equal(newQuestionName, updatedQuestion.Name); /* Check The Success */
    }
}