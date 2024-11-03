using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Common.Enums;
using Forms.Api.DAL.Memory;
using Xunit;

namespace Forms.API.DAL.IntegrationTests;

public class UserRepositoryTests
{
    private readonly IDatabaseFixture _dbFixture;
    public UserRepositoryTests()
    {
        _dbFixture = new InMemoryDatabaseFixture();
    }
    
    [Fact]
    public void GetById_Returns_Requested_User_Including_Their_Forms_With_Specific_Questions()
    {
        /* Arrange */ 
        var userRepository = _dbFixture.GetUserRepository();

        /* Act */
        var user = userRepository.GetById(_dbFixture.UserGuids[0]);

        /* Assert */
        Assert.Equal(_dbFixture.UserGuids[0], user.Id);
        Assert.Equal("John", user.FirstName);

        /* Check If User Have At Least One Form */
        Assert.NotEmpty(user.Forms);

        /* Find Form, That Has The Specific Question */
        var formWithQuestion1 = user.Forms.SingleOrDefault(f => 
            f.Id == _dbFixture.FormGuids[0] &&
            f.Questions.Any(q => q.Id == _dbFixture.QuestionGuids[0]));

        var formWithQuestion2 = user.Forms.SingleOrDefault(f => 
            f.Id == _dbFixture.FormGuids[0] &&
            f.Questions.Any(q => q.Id == _dbFixture.QuestionGuids[1]));

        /* Check If Forms And Question Exists */
        Assert.NotNull(formWithQuestion1);
        Assert.NotNull(formWithQuestion2);

        /* Check That Question Are In Form And Have Right Attributes */
        var question1 = formWithQuestion1.Questions.Single(q => q.Id == _dbFixture.QuestionGuids[0]);
        Assert.Equal("Otazka cislo jedna", question1.Name);
        Assert.Equal("Napis", question1.Description);
        Assert.Equal(QuestionType.OpenQuestion, question1.QuestionType);

        var question2 = formWithQuestion2.Questions.Single(q => q.Id == _dbFixture.QuestionGuids[1]);
        Assert.Equal("Otazka cislo dva", question2.Name);
        Assert.Equal("Napis 2", question2.Description);
        Assert.Equal(QuestionType.Selection, question2.QuestionType);
    }
    
    [Fact]
    public void Update_Saves_NewForm()
    {
        /* Arrange */
        var userRepository = _dbFixture.GetUserRepository();

        var userId = _dbFixture.UserGuids[0];
        var user = _dbFixture.GetUserDirectly(userId); // Použijeme tentýž kontext

        var newFormId = Guid.NewGuid();

        var newForm = new FormEntity
        {
            Id = newFormId,
            Name = "Dovolena vola",
            Description = "Vyber si dovolenou",
            DateOpen = DateTime.Now.AddDays(-5),
            DateClose = DateTime.Now.AddDays(30),
            UserId = userId
        };

        /* Act */
        user.Forms.Add(newForm);
        userRepository.Update(user); // Aktualizace s původním `DbContext`

        /* Assert */
        var userFromDb = userRepository.GetById(userId); // Stejný kontext pro ověření
        Assert.NotNull(userFromDb);
        Assert.Single(userFromDb.Forms, t => t.Id == newFormId);

        var formFromDb = _dbFixture.GetFormDirectly(newFormId);
        Assert.NotNull(formFromDb);
    }


    
    [Fact]
    public void RemoveForm_Removes_Form_From_User()
    {
        /* Arrange */
        var userRepository = _dbFixture.GetUserRepository();

        var userId = _dbFixture.UserGuids[0];
        var user = userRepository.GetById(userId);
        var formId = Guid.NewGuid();

        /* Add New Form To The User */
        var newForm = new FormEntity
        {
            Id = formId,
            Name = "Dovolena vola",
            Description = "Vyber si dovolenou",
            DateOpen = DateTime.Now.AddDays(-5),
            DateClose = DateTime.Now.AddDays(30),
            UserId = userId
        };

        /* Act */
        user.Forms.Add(newForm);
        
        /* Assert */
        var userFromDb = userRepository.GetById(userId);
        Assert.Contains(userFromDb.Forms, f => f.Id == formId);

        /* Act */
        userFromDb.Forms.Remove(newForm);
        userRepository.Update(userFromDb);

        /* Assert */
        userFromDb = userRepository.GetById(userId);
        Assert.NotNull(userFromDb);
    
        /* Check If Form Was Deleted */
        Assert.DoesNotContain(userFromDb.Forms, f => f.Id == formId);

        /* Removed Test Form From Database */
        var formFromDb = _dbFixture.GetFormDirectly(formId);
        Assert.Null(formFromDb);
    }   
    
    [Fact]
    public void UpdateForm_Changes_Form_Name_For_User()
    {
        /* Arrange */
        var userRepository = _dbFixture.GetUserRepository();
        var userId = _dbFixture.UserGuids[0];
        var user = _dbFixture.GetUserDirectly(userId);
        
        /* Check If User Does Have At Least One Form */
        Assert.NotNull(user);
        Assert.NotEmpty(user.Forms);

        /* Store First User's Form */ 
        var formToUpdate = user.Forms.First();
        var originalFormId = formToUpdate.Id;

        /* Update Form's Name And Store Changes */
        var newFormName = "Updated Name";
        formToUpdate.Name = newFormName;
        userRepository.Update(user);

        /* Act */
        var userFromDb = _dbFixture.GetUserDirectly(userId);

        /* Assert */
        Assert.NotNull(userFromDb);
        var updatedForm = userFromDb.Forms.Single(f => f.Id == originalFormId);
    
        /* Check The Success */
        Assert.Equal(newFormName, updatedForm.Name);
    }
}