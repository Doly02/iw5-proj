using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;

namespace Forms.API.DAL.IntegrationTests;

using System;
using System.Linq;
using Xunit;

public class UserSqlRepositoryTests : IClassFixture<SqlFixture>, IDisposable
{
    private readonly IUserRepository _userRepository;
    private readonly SqlFixture _fixture;

    public UserSqlRepositoryTests()
    {
        _fixture = new SqlFixture();
        _userRepository = _fixture.GetUserRepository();
    }

    [Fact]
    public void GetAll_ShouldReturnAllUsers()
    {
        // Act
        var users = _userRepository.GetAll();

        // Assert
        Assert.NotNull(users);
        Assert.Equal(_fixture.UserGuids.Count, users.Count);
    }

    [Fact]
    public void GetById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = _fixture.UserGuids.First();

        // Act
        var user = _userRepository.GetById(userId);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(userId, user.Id); 
    }

    [Fact]
    public void GetById_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var user = _userRepository.GetById(nonExistentId);

        // Assert
        Assert.Null(user); 
    }

    [Fact]
    public void Insert_ShouldAddNewUser()
    {
        // Arrange
        var newUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            FirstName = "New",
            LastName = "User",
            Email = "new.user@example.com",
            PhotoUrl = "https://i.ibb.co/ZdZ7rK8/user-3.jpg"
        };

        // Act
        var insertedId = _userRepository.Insert(newUser);

        // Assert
        var insertedUser = _userRepository.GetById(insertedId);
        Assert.NotNull(insertedUser);
        Assert.Equal(newUser.FirstName, insertedUser.FirstName);
    }

    [Fact]
    public void Update_ShouldModifyExistingUser()
    {
        // Arrange
        var userId = _fixture.UserGuids.First();
        var existingUser = _userRepository.GetById(userId);
        existingUser.FirstName = "UpdatedName";

        // Act
        var updatedId = _userRepository.Update(existingUser);

        // Assert
        var updatedUser = _userRepository.GetById(userId);
        Assert.NotNull(updatedUser);
        Assert.Equal("UpdatedName", updatedUser.FirstName);
    }

    [Fact]
    public void Update_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var nonExistentUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            FirstName = "Ghost",
            LastName = "User",
            Email = "ghost.user@example.com",
        };

        // Act
        var result = _userRepository.Update(nonExistentUser);

        // Assert
        Assert.Null(result); 
    }

    [Fact]
    public void Remove_ShouldDeleteUser()
    {
        // Arrange
        var userId = _fixture.UserGuids.First();
        Assert.True(_userRepository.Exists(userId)); 

        // Act
        _userRepository.Remove(userId);

        // Assert
        Assert.False(_userRepository.Exists(userId)); 
    }

    [Fact]
    public void Exists_ShouldReturnTrue_WhenUserExists()
    {
        // Arrange
        var userId = _fixture.UserGuids.First();

        // Act
        var exists = _userRepository.Exists(userId);

        // Assert
        Assert.True(exists); 
    }

    [Fact]
    public void Exists_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var exists = _userRepository.Exists(nonExistentId);

        // Assert
        Assert.False(exists); 
    }
    
    [Fact]
    public void CreateForm_ShouldAddFormToUser()
    {
        var formRepository = _fixture.GetFormRepository();
        
        // Arrange
        var userId = _fixture.UserGuids[0];
        var form = new FormEntity
        {
            Id = Guid.NewGuid(),
            Name = "Test Form",
            Description = "Test Description",
            DateOpen = DateTime.Now.AddDays(-1),
            DateClose = DateTime.Now.AddDays(30),
            UserId = userId
        };

        // Act
        formRepository.Insert(form);

        // Assert
        var user = _userRepository.GetById(userId);
        Assert.NotNull(user);
        Assert.Contains(user.Forms, f => f.Id == form.Id);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
