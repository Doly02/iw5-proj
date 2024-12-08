using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Castle.Components.DictionaryAdapter;
using Forms.Api.BL.Facades;
using Forms.Api.DAL.Common.Repositories;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Memory;
using Forms.Common.Models.Search;
using Forms.Common.Models.User;
using Moq;
using Xunit;

namespace Forms.Api.BL.UnitTests;

public class SearchFacadeTests
{
    [Fact]
    public async Task SearchFacade_Returns_Mapped_Results()
    {
        /* Arrange */
        var userRepositoryMock = new Mock<IUserRepository>(MockBehavior.Strict);
        var questionRepositoryMock = new Mock<IQuestionRepository>(MockBehavior.Strict);
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);
        var storage = new Storage();

        var users = new List<UserEntity>
        {
            new UserEntity
            {
                Id = storage.Users[0].Id,
                FirstName = storage.Users[0].FirstName,
                LastName = storage.Users[0].LastName,
                Email = storage.Users[0].Email,
            }
        };

        var mappedUserResults = new List<SearchResultModel>
        {
            new SearchResultModel
            {
                Id = users[0].Id,
                Type = "User",
                Name = $"{users[0].FirstName} {users[0].LastName}",
                Description = users[0].Email
            }
        };

        userRepositoryMock.Setup(repo => repo.SearchAsync(It.IsAny<string>())).ReturnsAsync(users);
        mapperMock.Setup(m => m.Map<List<SearchResultModel>>(users)).Returns(mappedUserResults);

        questionRepositoryMock.Setup(repo => repo.SearchAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<QuestionEntity>());
        mapperMock.Setup(m => m.Map<List<SearchResultModel>>(It.IsAny<List<QuestionEntity>>()))
            .Returns(new List<SearchResultModel>());
        mapperMock.Setup(m => m.Map<List<SearchResultModel>>(It.IsAny<List<FormEntity>>()))
            .Returns(new List<SearchResultModel>());

        var userFacade = new UserFacade(userRepositoryMock.Object, mapperMock.Object);
        var questionFacade = new QuestionFacade(questionRepositoryMock.Object, mapperMock.Object);


        var facade = new SearchFacade(
            userFacade,
            questionFacade);

        /* Act */
        var result = await facade.SearchAsync("John");

        /* Assert */
        Assert.NotNull(result);
        Assert.Equal(mappedUserResults, result);

        userRepositoryMock.Verify(repo => repo.SearchAsync("John"), Times.Once);
        questionRepositoryMock.Verify(repo => repo.SearchAsync("John"), Times.Once);
        mapperMock.Verify(m => m.Map<List<SearchResultModel>>(users), Times.Once);
        mapperMock.Verify(m => m.Map<List<SearchResultModel>>(It.IsAny<List<QuestionEntity>>()), Times.Once);
    }

    [Fact]
    public async Task SearchFacade_Returns_Mapped_Questions()
    {
        /* Arrange */
        var userRepositoryMock = new Mock<IUserRepository>(MockBehavior.Strict);
        var questionRepositoryMock = new Mock<IQuestionRepository>(MockBehavior.Strict);
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);
        var storage = new Storage();

        var questions = new List<QuestionEntity>
        {
            new QuestionEntity
            {
                Id = storage.Questions[0].Id,
                Name = storage.Questions[0].Name,
                Description = storage.Questions[0].Description,
                QuestionType = storage.Questions[0].QuestionType,
                FormId = storage.Questions[0].FormId
            }
        };

        var mappedQuestionResults = new List<SearchResultModel>
        {
            new SearchResultModel
            {
                Id = questions[0].Id,
                Type = "Question",
                Name = questions[0].Name,
                Description = questions[0].Description
            }
        };

        questionRepositoryMock.Setup(repo => repo.SearchAsync(It.IsAny<string>())).ReturnsAsync(questions);
        mapperMock.Setup(m => m.Map<List<SearchResultModel>>(questions)).Returns(mappedQuestionResults);

        userRepositoryMock.Setup(repo => repo.SearchAsync(It.IsAny<string>())).ReturnsAsync(new List<UserEntity>());
        mapperMock.Setup(m => m.Map<List<SearchResultModel>>(It.IsAny<List<UserEntity>>()))
            .Returns(new List<SearchResultModel>());

        var userFacade = new UserFacade(userRepositoryMock.Object, mapperMock.Object);
        var questionFacade = new QuestionFacade(questionRepositoryMock.Object, mapperMock.Object);

        var facade = new SearchFacade(userFacade, questionFacade);

        /* Act */
        var result = await facade.SearchAsync("Sample Question");

        /* Assert */
        Assert.NotNull(result);
        Assert.Equal(mappedQuestionResults, result);

        questionRepositoryMock.Verify(repo => repo.SearchAsync("Sample Question"), Times.Once);
        userRepositoryMock.Verify(repo => repo.SearchAsync("Sample Question"), Times.Once);
        mapperMock.Verify(m => m.Map<List<SearchResultModel>>(questions), Times.Once);
        mapperMock.Verify(m => m.Map<List<SearchResultModel>>(It.IsAny<List<UserEntity>>()), Times.Once);
    }
}