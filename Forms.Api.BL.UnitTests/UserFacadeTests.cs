using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Forms.Api.BL.Facades;
using Forms.Api.DAL.Common.Repositories;
using Forms.Api.DAL.Common.Entities;
using Forms.Common.Models.User;
using Forms.Api.DAL.Memory;
using Moq;
using Xunit;

namespace Forms.Api.BL.UnitTests;

public class UserFacadeTests
{
    [Fact]
    public void User_Delete_By_Correct_Method_On_Repository()
    {
        /* Arrange */
        var repositoryMock = new Mock<IUserRepository>(MockBehavior.Strict);
        repositoryMock.Setup(userRepo => userRepo.Remove(It.IsAny<Guid>()));

        var repository = repositoryMock.Object;
        var mapper = new Mock<IMapper>(MockBehavior.Strict).Object;
        var facade = new UserFacade(repository, mapper);

        var itemId = Guid.NewGuid();
        
        /* Act */
        facade.Delete(itemId);
        
        /* Assert */
        repositoryMock.Verify(userRepo => userRepo.Remove(itemId));
    }

    [Fact]
    public void GetAll_Returns_Mapped_Users()
    {
        /* Arrange */
        var repositoryMock = new Mock<IUserRepository>(MockBehavior.Strict);
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);

        var users = new List<UserEntity>
        {
            new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PasswordHash = "hashed_password"
            }
        };

        var mappedUsers = new List<UserListModel>
        {
            new UserListModel
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhotoUrl = "https://example.com/photo.jpg"
            }
        };

        repositoryMock.Setup(repo => repo.GetAll()).Returns(users);
        mapperMock.Setup(m => m.Map<List<UserListModel>>(users)).Returns(mappedUsers);

        var facade = new UserFacade(repositoryMock.Object, mapperMock.Object);

        /* Act */
        var result = facade.GetAll();

        /* Assert */
        Assert.Equal(mappedUsers, result);
        repositoryMock.Verify(repo => repo.GetAll(), Times.Once);
        mapperMock.Verify(m => m.Map<List<UserListModel>>(users), Times.Once);
    }

    [Fact]
    public void Create_Calls_Insert_On_Repository_With_Mapped_Entity()
    {
        /* Arrange */
        var repositoryMock = new Mock<IUserRepository>(MockBehavior.Strict);
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);
    
        var userModel = new UserDetailModel
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Novak",
            Email = "john.novak@example.com",
            PasswordHash = "hashed_password",
        };
        
        var userEntity = new UserEntity
        {
            Id = userModel.Id, 
            FirstName = userModel.FirstName,
            LastName = userModel.LastName,
            Email = userModel.Email,
            PasswordHash = userModel.PasswordHash,
        };
        
        repositoryMock.Setup(repo => repo.Insert(userEntity)).Returns(userEntity.Id);
        mapperMock.Setup(m => m.Map<UserEntity>(userModel)).Returns(userEntity);
        var facade = new UserFacade(repositoryMock.Object, mapperMock.Object);

        /* Act */
        var result = facade.Create(userModel);

        /* Assert */
        Assert.Equal(userEntity.Id, result);
        repositoryMock.Verify(repo => repo.Insert(It.Is<UserEntity>(user =>
            user.Id == userModel.Id &&
            user.FirstName == userModel.FirstName &&
            user.LastName == userModel.LastName &&
            user.Email == userModel.Email &&
            user.PasswordHash == userModel.PasswordHash
        )), Times.Once);
        mapperMock.Verify(m => m.Map<UserEntity>(userModel), Times.Once);
    }

    [Fact]
    public void Update_Calls_Update_On_Repository_With_Mapped_Entity()
    {
        /* Arrange */
        var repositoryMock = new Mock<IUserRepository>(MockBehavior.Strict);
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);

        var userModel = new UserDetailModel
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PasswordHash = "hashed_password"
        };

        var userEntity = new UserEntity
        {
            Id = userModel.Id,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PasswordHash = "hashed_password"
        };

        repositoryMock.Setup(repo => repo.Update(userEntity)).Returns(userModel.Id);
        mapperMock.Setup(m => m.Map<UserEntity>(userModel)).Returns(userEntity);

        var facade = new UserFacade(repositoryMock.Object, mapperMock.Object);

        /* Act */
        var result = facade.Update(userModel);

        /* Assert */
        Assert.Equal(userModel.Id, result);
        repositoryMock.Verify(repo => repo.Update(userEntity), Times.Once);
        mapperMock.Verify(m => m.Map<UserEntity>(userModel), Times.Once);
    }

    private static UserFacade GetUserFacadeWithForbiddenDependencyCalls()
    {
        var repository = new Mock<IUserRepository>(MockBehavior.Strict).Object;
        var mapper = new Mock<IMapper>(MockBehavior.Strict).Object;
        var facade = new UserFacade(repository, mapper);
        return facade;
    }
}
