using AutoMapper;
using Forms.Api.BL.Facades;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;
using Forms.Api.DAL.Memory;
using Forms.Common.Enums;
using Forms.Common.Models.Question;
using Moq;
using Xunit;

namespace Forms.Api.BL.UnitTests;

public class QuestionFacadeTests
{
    [Fact]
    public void Question_Delete_By_Correct_Method_On_Repository()
    {
        // arrange
        var repositoryMock = new Mock<IQuestionRepository>(MockBehavior.Strict);
        repositoryMock.Setup(questionRepository => questionRepository.Remove(It.IsAny<Guid>()));
        
        var repository = repositoryMock.Object;
        var mapper = new Mock<IMapper>(MockBehavior.Strict).Object;
        var facade = new QuestionFacade(repository, mapper);

        var itemId = Guid.NewGuid();
        
        // act
        facade.Delete(itemId);
        
        // assert
        repositoryMock.Verify(questionRepository => questionRepository.Remove(itemId));
    }
    
    [Fact]
    public void GetAll_Returns_Mapped_Questions()
    {
        // arrange
        var repositoryMock = new Mock<IQuestionRepository>(MockBehavior.Strict);
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);
        var storage = new Storage();
        var questions = storage.Questions.ToList();  // seed data
        var mappedQuestions = questions.Select(q => new QuestionListModel
        {
            Name = q.Name,
            Description = q.Description,
            QuestionType = q.QuestionType,
            Answer = q.Answer
        }).ToList();

        
        repositoryMock.Setup(questionRepository => questionRepository.GetAll()).Returns(questions);
        mapperMock.Setup(m => m.Map<List<QuestionListModel>>(questions)).Returns(mappedQuestions);

        var facade = new QuestionFacade(repositoryMock.Object, mapperMock.Object);
        
        // act
        var result = facade.GetAll();  
        
        // assert
        Assert.Equal(mappedQuestions, result); 
        repositoryMock.Verify(questionRepository => questionRepository.GetAll(), Times.Once);  
        mapperMock.Verify(m => m.Map<List<QuestionListModel>>(questions), Times.Once); 
    }
    
    [Fact]
    public void Update_Calls_Update_On_Repository_With_Mapped_Entity()
    {
        // arrange
        var repositoryMock = new Mock<IQuestionRepository>(MockBehavior.Strict);
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);

        var storage = new Storage();
        
        var questionModel = new QuestionDetailModel
        {
            Id = storage.Questions[0].Id,
            Name = "Do you love dogs?", 
            Description = "Choose one answer.", 
            Answer = new List<string> { "Yes", "No"},
            QuestionType = QuestionType.Selection
        };
        
        var updatedQuestionEntity = new QuestionEntity
        {
            Id = questionModel.Id,
            Name = questionModel.Name,
            Description = questionModel.Description,
            QuestionType = QuestionType.Selection,
            Answer = questionModel.Answer,
            FormId = storage.Forms[0].Id
        };
        
        // arrange mock
        repositoryMock.Setup(questionRepository => questionRepository.Update(It.IsAny<QuestionEntity>())).Returns(questionModel.Id);
        mapperMock.Setup(m => m.Map<QuestionEntity>(questionModel)).Returns(updatedQuestionEntity);

        var facade = new QuestionFacade(repositoryMock.Object, mapperMock.Object);
        
        // act
        var result = facade.Update(questionModel);
        
        // assert
        Assert.Equal(questionModel.Id, result);
        
        repositoryMock.Verify(questionRepository => questionRepository.Update(It.Is<QuestionEntity>(q => 
            q.Answer != null &&
            q.Id == updatedQuestionEntity.Id &&
            q.Name == "Do you love dogs?" &&
            q.Description == "Choose one answer." &&
            q.Answer.SequenceEqual(new List<string> { "Yes", "No" }) &&
            q.QuestionType == QuestionType.Selection
        )), Times.Once);
        
        mapperMock.Verify(m => m.Map<QuestionEntity>(questionModel), Times.Once);
    }
    
    [Fact]
    public void GetById_Returns_Mapped_QuestionDetailModel()
    {
        // arrange
        var repositoryMock = new Mock<IQuestionRepository>(MockBehavior.Strict);
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);

        var storage = new Storage();
        var questionId = storage.Questions[0].Id;
        var questionEntity = storage.Questions[0];
        
        var expectedQuestionDetailModel = new QuestionDetailModel
        {
            Id = questionEntity.Id,
            Name = questionEntity.Name,
            Description = questionEntity.Description,
            Answer = questionEntity.Answer,
            QuestionType = questionEntity.QuestionType
        };
        
        repositoryMock.Setup(questionRepository => questionRepository.GetById(questionId)).Returns(questionEntity);
        mapperMock.Setup(m => m.Map<QuestionDetailModel>(questionEntity)).Returns(expectedQuestionDetailModel);
        var facade = new QuestionFacade(repositoryMock.Object, mapperMock.Object);

        // act
        var result = facade.GetById(questionId);

        // assert
        Assert.Equal(expectedQuestionDetailModel, result); 
        repositoryMock.Verify(questionRepository => questionRepository.GetById(questionId), Times.Once); 
        mapperMock.Verify(m => m.Map<QuestionDetailModel>(questionEntity), Times.Once); 
    }
    
    [Fact]
    public void Create_Calls_Insert_On_Repository_With_Mapped_Entity()
    {
        // arrange
        var repositoryMock = new Mock<IQuestionRepository>(MockBehavior.Strict);
        var mapperMock = new Mock<IMapper>(MockBehavior.Strict);

        var storage = new Storage();
        
        var questionModel = new QuestionDetailModel
        {
            Id = Guid.NewGuid(),
            Name = "New Question",
            Description = "New Question Description",
            Answer = new List<string> { "New1", "New2", "New3", "New4"},
            QuestionType = QuestionType.Range
        };
        
        var questionEntity = new QuestionEntity
        {
            Id = questionModel.Id, 
            Name = questionModel.Name,
            Description = questionModel.Description,
            QuestionType = QuestionType.Range,
            Answer = questionModel.Answer,
            FormId = storage.Forms[0].Id
        };
        
        repositoryMock.Setup(questionRepository => questionRepository.Insert(questionEntity)).Returns(questionEntity.Id);
        mapperMock.Setup(m => m.Map<QuestionEntity>(questionModel)).Returns(questionEntity);
        var facade = new QuestionFacade(repositoryMock.Object, mapperMock.Object);

        // act
        var result = facade.Create(questionModel);

        // assert
        Assert.Equal(questionEntity.Id, result);
        repositoryMock.Verify(questionRepository => questionRepository.Insert(It.Is<QuestionEntity>(q =>
            q.Id == questionModel.Id &&
            q.Name == questionModel.Name &&
            q.Description == questionModel.Description &&
            q.Answer == questionModel.Answer &&
            q.QuestionType == QuestionType.Range
        )), Times.Once);
        mapperMock.Verify(m => m.Map<QuestionEntity>(questionModel), Times.Once);
    }
}