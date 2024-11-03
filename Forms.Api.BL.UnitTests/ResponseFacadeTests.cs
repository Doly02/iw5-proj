using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Forms.Api.BL.Facades;
using Forms.Api.DAL.Common.Repositories;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Memory;
using Forms.Common.Models.Question;
using Forms.Common.Models.Response;
using Forms.Common.Models.User;
using Moq;
using Xunit;

namespace Forms.Api.BL.UnitTests;

public class ResponseFacadeTests
{

    [Fact]
    public void Response_Delete_By_Correct_Method_On_Repository()
    {
        // arrange
        var repositoryMock = new Mock<IResponseRepository>(MockBehavior.Strict);
        repositoryMock.Setup(responseRepository => responseRepository.Remove(It.IsAny<Guid>()));

        var repository = repositoryMock.Object;
        var mapper = new Mock<IMapper>(MockBehavior.Strict).Object;
        var facade = new ResponseFacade(repository, mapper);

        var itemId = Guid.NewGuid();
        // act
        facade.Delete(itemId);

        // assert
        repositoryMock.Verify(responseRepository => responseRepository.Remove(itemId));
    }
    
    [Fact]
    public void GetAll_Returns_Mapped_Responses()
    {
        /* Arrange */
        var repositoryMock = new Mock<IResponseRepository>(MockBehavior.Strict);
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);
        var storage = new Storage();
        var responses = storage.Responses.ToList();  // Formuláře ze seed dat
        
        var mappedResponses = responses.Select(r => new ResponseListModel
        {
            Id = r.Id,
            User = r.User,
            Question = r.Question,
            UserResponse = r.UserResponse
        }).ToList();

        repositoryMock.Setup(repo => repo.GetAll()).Returns(responses);
        mapperMock.Setup(m => m.Map<List<ResponseListModel>>(responses)).Returns(mappedResponses);

        var facade = new ResponseFacade(repositoryMock.Object, mapperMock.Object);

        // Act
        var result = facade.GetAll();  

        // Assert
        Assert.Equal(mappedResponses, result); 
        repositoryMock.Verify(repo => repo.GetAll(), Times.Once);  
        mapperMock.Verify(m => m.Map<List<ResponseListModel>>(responses), Times.Once); 
    }
    
    [Fact]
    public void Update_Calls_Update_On_Repository_With_Mapped_Entity()
    {
        // arrange
        var repositoryMock = new Mock<IResponseRepository>(MockBehavior.Strict);
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);

        var storage = new Storage();
        
        var responseModel = new ResponseDetailModel
        {
            Id = storage.Responses[0].Id,
            Question = new QuestionListModel
            {
                Id = storage.Questions[0].Id,
                Name = storage.Questions[0].Name,
                Description = storage.Questions[0].Description,
                Answer = storage.Questions[0].Answer,
                QuestionType = storage.Questions[0].QuestionType,
            },
            User = new UserListModel
            {
                Id = storage.Users[0].Id,
                FirstName = storage.Users[0].FirstName,
                LastName = storage.Users[0].LastName,
                Email = storage.Users[0].Email
            },
            UserResponse = new List<string> { "testUpdatedResponse" }
        };
        
        var updatedResponseEntity = new ResponseEntity
        {
            Id = responseModel.Id,
            User = storage.Users[0],
            Question = storage.Questions[0],
            UserResponse = responseModel.UserResponse
        };

        // arrange mock
        repositoryMock.Setup(repo => repo.Update(updatedResponseEntity)).Returns(responseModel.Id);
        mapperMock.Setup(m => m.Map<ResponseEntity>(responseModel)).Returns(updatedResponseEntity);

        var facade = new ResponseFacade(repositoryMock.Object, mapperMock.Object);

        // act
        var result = facade.Update(responseModel);

        // assert
        Assert.Equal(responseModel.Id, result);
    
        repositoryMock.Verify(repo => repo.Update(It.Is<ResponseEntity>(f => 
            f.Id == updatedResponseEntity.Id &&
            f.User == updatedResponseEntity.User &&
            f.Question == updatedResponseEntity.Question &&
            f.UserResponse != null &&
            f.UserResponse.Contains("testUpdatedResponse")
        )), Times.Once);
    
        mapperMock.Verify(m => m.Map<ResponseEntity>(responseModel), Times.Once);
    }

    [Fact]
    public void Create_Calls_Insert_On_Repository_With_Mapped_Entity()
    {
        // arrange
        var repositoryMock = new Mock<IResponseRepository>(MockBehavior.Strict);
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);

        var storage = new Storage();

        var responseModel = new ResponseDetailModel
        {
            Id = Guid.NewGuid(),
            Question = new QuestionListModel
            {
                Id = storage.Questions[0].Id,
                Name = storage.Questions[0].Name,
                Description = storage.Questions[0].Description,
                Answer = storage.Questions[0].Answer,
                QuestionType = storage.Questions[0].QuestionType,
            },
            User = new UserListModel
            {
                Id = storage.Users[0].Id,
                FirstName = storage.Users[0].FirstName,
                LastName = storage.Users[0].LastName,
                Email = storage.Users[0].Email
            },
            UserResponse = new List<string> { "testCreatedResponse" }
        };
        
        var responseEntity = new ResponseEntity
        {
            Id = responseModel.Id, 
            QuestionId = storage.Questions[0].Id,
            Question = storage.Questions[0],
            User = storage.Users[0],
            UserId = responseModel.User.Id,
            UserResponse = responseModel.UserResponse
        };
        
        repositoryMock.Setup(repo => repo.Insert(responseEntity)).Returns(responseEntity.Id);
        mapperMock.Setup(m => m.Map<ResponseEntity>(responseModel)).Returns(responseEntity);
        var facade = new ResponseFacade(repositoryMock.Object, mapperMock.Object);

        // act
        var result = facade.Create(responseModel);

        // assert
        Assert.Equal(responseEntity.Id, result);
        repositoryMock.Verify(repo => repo.Insert(It.Is<ResponseEntity>(f =>
            f.Id == responseModel.Id &&
            f.UserId == responseModel.User.Id &&
            f.UserResponse != null &&
            f.UserResponse.SequenceEqual(responseModel.UserResponse)
        )), Times.Once);
        
        
        mapperMock.Verify(m => m.Map<ResponseEntity>(responseModel), Times.Once);
    }
  

    private static ResponseFacade GetResponseFacadeWithForbiddenDependencyCalls()
    {
        var repository = new Mock<IResponseRepository>(MockBehavior.Strict).Object;
        var mapper = new Mock<IMapper>(MockBehavior.Strict).Object;
        var facade = new ResponseFacade(repository, mapper);
        return facade;
    }
}
