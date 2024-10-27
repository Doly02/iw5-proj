using AutoMapper;
using Forms.Api.BL.MapperProfiles;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;
using Forms.Api.DAL.EF;
using Forms.Api.DAL.EF.Repositories;
using Forms.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Forms.API.DAL.IntegrationTests;

public class SqlDatabaseFixture : IDatabaseFixture, IDisposable
{
    private readonly DbContextOptions<FormsDbContext> _dbContextOptions;
    private readonly FormsDbContext _context;
    private static readonly IMapper _mapper = InitializeMapper();

    public SqlDatabaseFixture()
    {
        // Nastavení in-memory SQLite databáze pro testování
        DbContextOptionsBuilder<FormsDbContext> builder = new();
        _dbContextOptions = builder.UseSqlite("Data Source=:memory:") // SQLite in-memory database
            .Options;

        // Inicializace kontextu a otevření spojení
        _context = new FormsDbContext(_dbContextOptions);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        SeedDatabase(_context);
    }
    
    private static IMapper InitializeMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserMapperProfile>();
            cfg.AddProfile<FormMapperProfile>();
            cfg.AddProfile<ResponseMapperProfile>();
            cfg.AddProfile<QuestionMapperProfile>();
        });
        return config.CreateMapper();
    }

    private void SeedDatabase(FormsDbContext context)
    {
        // Seedování dat do tabulek uživatelů
        var user1 = new UserEntity
        {
            Id = UserGuids[0],
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PasswordHash = "hashedPassword123",
            PhotoUrl = "https://i.ibb.co/ZdZ7rK8/user-1.jpg"
        };

        var user2 = new UserEntity
        {
            Id = UserGuids[1],
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            PasswordHash = "hashedPassword456",
            PhotoUrl = "https://i.ibb.co/ZdZ7rK8/user-2.jpg"
        };

        context.Users.AddRange(user1, user2);

        // Seedování formulářů a otázek
        var form1 = new FormEntity
        {
            Id = FormGuids[0],
            Name = "VUT FIT",
            Description = "Bud FIT",
            DateOpen = DateTime.Now.AddDays(-5),
            DateClose = DateTime.Now.AddDays(30),
            UserId = user1.Id
        };

        var form2 = new FormEntity
        {
            Id = FormGuids[1],
            Name = "Novy formular",
            Description = "Formular pro testovani",
            DateOpen = DateTime.Now.AddDays(-3),
            DateClose = DateTime.Now.AddDays(30),
            UserId = user1.Id
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

        context.Forms.AddRange(form1, form2);
        context.Questions.AddRange(question1, question2);

        context.SaveChanges();
    }

    public UserEntity? GetUserDirectly(Guid userId)
    {
        return DeepClone(_context.Users
            .Include(u => u.Forms)
            .ThenInclude(f => f.Questions)
            .AsNoTracking()
            .SingleOrDefault(u => u.Id == userId));
    }

    public QuestionEntity? GetQuestionDirect(Guid questionId)
    {
        return DeepClone(_context.Questions
            .AsNoTracking()
            .SingleOrDefault(q => q.Id == questionId));
    }


    public FormEntity? GetFormDirectly(Guid formId)
    {
        return DeepClone(_context.Forms
            .Include(f => f.Questions)
            .AsNoTracking()
            .SingleOrDefault(f => f.Id == formId));
    }

    public IUserRepository GetUserRepository()
    {
        return new UserRepository(_context, _mapper); // Konfigurace vašeho repository s kontextem
    }
    
    private T DeepClone<T>(T input)
    {
        var json = JsonConvert.SerializeObject(input);
        return JsonConvert.DeserializeObject<T>(json);
    }

    // Guids pro identifikaci entit
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

    public IList<Guid> UserGuids { get; } = new List<Guid>
    {
        new("99199199-3223-4ff4-aabb-3333cc2cbf2e"),
        new("89199199-3223-4ff4-aabb-3333cc2cbf11")
    };

    public void Dispose()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }
}
