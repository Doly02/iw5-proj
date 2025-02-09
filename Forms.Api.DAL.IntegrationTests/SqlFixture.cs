using AutoMapper;
using Forms.Api.BL.MapperProfiles;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;
using Forms.Api.DAL.EF;
using Forms.Api.DAL.EF.Repositories;
using Forms.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Forms.API.DAL.IntegrationTests;

public class SqlFixture : IDatabaseFixture, IDisposable
{
    public readonly FormsDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IFormRepository _formRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IResponseRepository _responseRepository;
    private readonly string _databaseName;

    public SqlFixture()
    {
        _databaseName = $"FormsTestDb_{Guid.NewGuid()}";

        var connectionString = $"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog={_databaseName};Integrated Security=True;";

        var options = new DbContextOptionsBuilder<FormsDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "DALSelectionOptions:SeedDemoData", "true" }, 
                { "DALSelectionOptions:RecreateDatabaseEachTime", "true" } 
            }!)
            .Build();

        _context = new FormsDbContext(options, configuration);

        _userRepository = new UserRepository(_context, new MapperConfiguration(cfg => cfg.AddProfile<UserMapperProfile>()).CreateMapper());
        _formRepository = new FormRepository(_context,
            new MapperConfiguration(cfg => cfg.AddProfile<FormEntityMapperProfile>()).CreateMapper());
        _questionRepository = new QuestionRepository(_context,
            new MapperConfiguration(cfg => cfg.AddProfile<QuestionMapperProfile>()).CreateMapper());
        _responseRepository = new ResponseRepository(_context, new MapperConfiguration(cfg => cfg.AddProfile<ResponseMapperProfile>()).CreateMapper());

        CreateDatabase();
        SeedDatabase();
    }

    private void CreateDatabase()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    private void SeedDatabase()
    {
        var user1 = new UserEntity
        {
            Id = UserGuids[0],
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PhotoUrl = "https://i.ibb.co/ZdZ7rK8/user-1.jpg",
            Forms = new List<FormEntity>()
        };

        var user2 = new UserEntity
        {
            Id = UserGuids[1],
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            PhotoUrl = "https://i.ibb.co/ZdZ7rK8/user-2.jpg",
            Forms = new List<FormEntity>()
        };

        var form1 = new FormEntity
        {
            Id = FormGuids[0],
            Name = "VUT FIT",
            Description = "Bud FIT",
            DateOpen = DateTime.Now.AddDays(-5),
            DateClose = DateTime.Now.AddDays(30),
            UserId = user1.Id,
            Questions = new List<QuestionEntity>()
        };
        
        var form2 = new FormEntity
        {
            Id = FormGuids[1],
            Name = "Dovolena vola",
            Description = "Odpocin si konecne ve sve vysnene destinaci!",
            DateOpen = DateTime.Now.AddDays(-4),
            DateClose = DateTime.Now.AddDays(27),
            UserId = user2.Id,
            Questions = new List<QuestionEntity>()
        };
        
        var question1 = new QuestionEntity
        {
            Id = QuestionGuids[0],
            Name = "Otazka cislo jedna",
            Description = "Napis",
            QuestionType = QuestionType.OpenQuestion,
            FormId = form1.Id
        };

        var question2 = new QuestionEntity
        {
            Id = QuestionGuids[1],
            Name = "Otazka cislo dva",
            Description = "Napis 2",
            QuestionType = QuestionType.Selection,
            FormId = form1.Id
        };

        var response1 = new ResponseEntity
        {
            Id = ResponseGuids[0],
            Question = question1,
            QuestionId = question1.Id,
            User = user1,
            UserId = user1.Id,
            UserResponse = ["Napisem ahoj!"]
        };
        
        var response2 = new ResponseEntity
        {
            Id = ResponseGuids[1],
            Question = question2,
            QuestionId = question2.Id,
            User = user1,
            UserId = user1.Id,
            UserResponse = ["Ano"]
        };

        form1.Questions.Add(question1);
        form1.Questions.Add(question2);
        user1.Forms.Add(form1);
        
        // user1.Responses.Add(response1);
        question1.Responses.Add(response1);
        
        // user1.Responses.Add(response2);
        question2.Responses.Add(response2);

        _context.Users.AddRange(user1, user2);
        _context.Forms.Add(form1);
        _context.Forms.Add(form2);
        _context.Questions.AddRange(question1, question2);
        _context.Responses.AddRange(response1, response2);
        _context.SaveChanges();
    }

    public QuestionEntity? GetQuestionDirectly(Guid questionId) =>
        _context.Questions.AsNoTracking().SingleOrDefault(q => q.Id == questionId);

    public UserEntity? GetUserDirectly(Guid userId) =>
        _context.Users.AsNoTracking().SingleOrDefault(u => u.Id == userId);

    public FormEntity? GetFormDirectly(Guid formId) =>
        _context.Forms.AsNoTracking().SingleOrDefault(f => f.Id == formId);
    public ResponseEntity? GetResponseDirectly(Guid responseId) =>
        _context.Responses.AsNoTracking().SingleOrDefault(f => f.Id == responseId);

    
    public IUserRepository GetUserRepository() => _userRepository;
    public IFormRepository GetFormRepository() => _formRepository;
    public IQuestionRepository GetQuestionRepository() => _questionRepository;
    public IResponseRepository GetResponseRepository() => _responseRepository;

    public IList<Guid> QuestionGuids { get; } = new List<Guid>
    {
        new("23b19020-8709-1010-a200-11397aa416dc"),
        new("23b3001e-4a2e-4010-33f0-2243aaf59238")
    };

    public IList<Guid> FormGuids { get; } = new List<Guid>
    {
        new("001000cd-44f4-4f44-aabb-3d96cc2cbf2e"),
        new("111000cd-44f4-4f44-aabb-3d96cc2cbf1f")
    };

    public IList<Guid> ResponseGuids { get; } = new List<Guid>
    {
        new("222000cd-44f4-4f44-aabb-3d96cc2cbf2a"),
        new("333000cd-44f4-4f44-aabb-3d96cc2cbf3b")
    };

    public IList<Guid> UserGuids { get; } = new List<Guid>
    {
        new("99199199-3223-4ff4-aabb-3333cc2cbf2e"),
        new("89199199-3223-4ff4-aabb-3333cc2cbf11")
    };

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
