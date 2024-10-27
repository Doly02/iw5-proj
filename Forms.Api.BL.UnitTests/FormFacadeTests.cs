using AutoMapper;
using Forms.Api.BL.Facades;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;
using Forms.Api.DAL.Memory;
using Forms.Common.Models.Form;
using Forms.Common.Models.User;
using Moq;
using Xunit;

namespace Forms.Api.BL.UnitTests;

public class FormFacadeTests
{
    [Fact]
    public void Form_Delete_By_Correct_Method_On_Repository()
    {
        // arrange
        var repositoryMock = new Mock<IFormRepository>(MockBehavior.Strict);
        repositoryMock.Setup(formRepository => formRepository.Remove(It.IsAny<Guid>()));

        var repository = repositoryMock.Object;
        var mapper = new Mock<IMapper>(MockBehavior.Strict).Object;
        var facade = new FormFacade(repository, mapper);

        var itemId = Guid.NewGuid();
        // act
        facade.Delete(itemId);

        // assert
        repositoryMock.Verify(formRepository => formRepository.Remove(itemId));
    }

    [Fact]
    public void GetAll_Returns_Mapped_Forms()
    {
        // arrange
        var repositoryMock = new Mock<IFormRepository>(MockBehavior.Strict);
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);
        var storage = new Storage();
        var forms = storage.Forms.ToList();  // Formuláře ze seed dat
        var mappedForms = forms.Select(f => new FormListModel
        {
            Id = f.Id,
            Name = f.Name,
            Description = f.Description,
            DateOpen = f.DateOpen,
            DateClose = f.DateClose
        }).ToList();

        
        repositoryMock.Setup(repo => repo.GetAll()).Returns(forms);
        mapperMock.Setup(m => m.Map<List<FormListModel>>(forms)).Returns(mappedForms);

        var facade = new FormFacade(repositoryMock.Object, mapperMock.Object);

        // Act
        var result = facade.GetAll();  

        // Assert
        Assert.Equal(mappedForms, result); 
        repositoryMock.Verify(repo => repo.GetAll(), Times.Once);  
        mapperMock.Verify(m => m.Map<List<FormListModel>>(forms), Times.Once); 
    }
    
    [Fact]
    public void Update_Calls_Update_On_Repository_With_Mapped_Entity()
    {
        // arrange
        var repositoryMock = new Mock<IFormRepository>(MockBehavior.Strict);
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);

        var storage = new Storage();
        
        var formModel = new FormDetailModel
        {
            Id = storage.Forms[0].Id,
            Name = "Updated Form Name", 
            Description = "Updated Description", 
            DateOpen = storage.Forms[0].DateOpen,
            DateClose = storage.Forms[0].DateClose,
            User = new UserListModel
            {
                Id = storage.Users[0].Id,
                FirstName = storage.Users[0].FirstName,
                LastName = storage.Users[0].LastName,
                Email = storage.Users[0].Email
            },
            UserId = storage.Users[0].Id
        };
        
        var updatedFormEntity = new FormEntity
        {
            Id = formModel.Id,
            Name = formModel.Name, 
            Description = formModel.Description, 
            DateOpen = formModel.DateOpen,
            DateClose = formModel.DateClose,
            UserId = storage.Users[0].Id
        };

        // arrange mock
        repositoryMock.Setup(repo => repo.Update(updatedFormEntity)).Returns(formModel.Id);
        mapperMock.Setup(m => m.Map<FormEntity>(formModel)).Returns(updatedFormEntity);

        var facade = new FormFacade(repositoryMock.Object, mapperMock.Object);

        // act
        var result = facade.Update(formModel);

        // assert
        Assert.Equal(formModel.Id, result);
    
        repositoryMock.Verify(repo => repo.Update(It.Is<FormEntity>(f => 
            f.Id == updatedFormEntity.Id &&
            f.Name == "Updated Form Name" &&
            f.Description == "Updated Description"
        )), Times.Once);
    
        mapperMock.Verify(m => m.Map<FormEntity>(formModel), Times.Once);
    }
    
    [Fact]
    public void GetById_Returns_Mapped_FormDetailModel()
    {
        // arrange
        var repositoryMock = new Mock<IFormRepository>(MockBehavior.Strict);
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);

        var storage = new Storage();
        var formId = storage.Forms[0].Id;
        var formEntity = storage.Forms[0];
        
        var expectedFormDetailModel = new FormDetailModel
        {
            Id = formEntity.Id,
            Name = formEntity.Name,
            Description = formEntity.Description,
            DateOpen = formEntity.DateOpen,
            DateClose = formEntity.DateClose,
            User = new UserListModel
            {
                Id = storage.Users[0].Id,
                FirstName = storage.Users[0].FirstName,
                LastName = storage.Users[0].LastName,
                Email = storage.Users[0].Email
            },
            UserId = storage.Users[0].Id
        };
        
        repositoryMock.Setup(repo => repo.GetById(formId)).Returns(formEntity);
        mapperMock.Setup(m => m.Map<FormDetailModel>(formEntity)).Returns(expectedFormDetailModel);
        var facade = new FormFacade(repositoryMock.Object, mapperMock.Object);

        // act
        var result = facade.GetById(formId);

        // assert
        Assert.Equal(expectedFormDetailModel, result); 
        repositoryMock.Verify(repo => repo.GetById(formId), Times.Once); 
        mapperMock.Verify(m => m.Map<FormDetailModel>(formEntity), Times.Once); 
    }

    [Fact]
    public void Create_Calls_Insert_On_Repository_With_Mapped_Entity()
    {
        // arrange
        var repositoryMock = new Mock<IFormRepository>(MockBehavior.Strict);
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);

        var storage = new Storage();
        
        var formModel = new FormDetailModel
        {
            Id = Guid.NewGuid(),
            Name = "New Form",
            Description = "New Form Description",
            DateOpen = DateTime.Now,
            DateClose = DateTime.Now.AddDays(30),
            User = new UserListModel
            {
                Id = storage.Users[0].Id,
                FirstName = storage.Users[0].FirstName,
                LastName = storage.Users[0].LastName,
                Email = storage.Users[0].Email
            },
            UserId = storage.Users[0].Id
        };
        
        var formEntity = new FormEntity
        {
            Id = formModel.Id, 
            Name = formModel.Name,
            Description = formModel.Description,
            DateOpen = formModel.DateOpen,
            DateClose = formModel.DateClose,
            UserId = formModel.User.Id
        };
        
        repositoryMock.Setup(repo => repo.Insert(formEntity)).Returns(formEntity.Id);
        mapperMock.Setup(m => m.Map<FormEntity>(formModel)).Returns(formEntity);
        var facade = new FormFacade(repositoryMock.Object, mapperMock.Object);

        // act
        var result = facade.Create(formModel);

        // assert
        Assert.Equal(formEntity.Id, result);
        repositoryMock.Verify(repo => repo.Insert(It.Is<FormEntity>(f =>
            f.Id == formModel.Id &&
            f.Name == formModel.Name &&
            f.Description == formModel.Description &&
            f.DateOpen == formModel.DateOpen &&
            f.DateClose == formModel.DateClose &&
            f.UserId == formModel.User.Id
        )), Times.Once);
        mapperMock.Verify(m => m.Map<FormEntity>(formModel), Times.Once);
    }


}