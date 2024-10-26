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
        Assert.Equal("VUT FIT", form.Name);
        Assert.Equal("Bud FIT", form.Description);

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
}