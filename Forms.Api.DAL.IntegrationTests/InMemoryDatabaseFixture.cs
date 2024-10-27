using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;
using Forms.Api.DAL.Memory;
using Forms.Api.DAL.Memory.Repositories;
using Forms.Common.Enums;
using Newtonsoft.Json;

namespace Forms.API.DAL.IntegrationTests;

public class InMemoryDatabaseFixture : IDatabaseFixture
{
    private readonly Lazy<Storage> _inMemoryStorage;
    
    public InMemoryDatabaseFixture()
    {
        _inMemoryStorage = new Lazy<Storage>(CreateInMemoryStorage);
    }
    
    private T DeepClone<T>(T input)
    {
        var json = JsonConvert.SerializeObject(input);
        var result = JsonConvert.DeserializeObject<T>(json);
    
        if (null == result)
        {
            throw new InvalidOperationException("Deserialization resulted in a null object.");
        }
    
        return result;
    }
    
    public QuestionEntity? GetQuestionDirectly(Guid questionId)
    {
        var question = _inMemoryStorage.Value.Questions.SingleOrDefault(q => q.Id == questionId);
        if (null == question)
        {
            return null;
        }
        return DeepClone(question);
    }

    public UserEntity? GetUserDirectly(Guid userId)
    {
        var user = _inMemoryStorage.Value.Users.SingleOrDefault(t => t.Id == userId);
        if (null == user)
        {
            return null;
        }
        return DeepClone(user);
    }

    public FormEntity? GetFormDirectly(Guid formId)
    {
        var form = _inMemoryStorage.Value.Forms.SingleOrDefault(t => t.Id == formId);
        if (null == form)
        {
            return null;
        }
        return DeepClone(form);
    }
    
    public ResponseEntity? GetResponseDirectly(Guid responseId)
    {
        var response = _inMemoryStorage.Value.Responses.SingleOrDefault(t => t.Id == responseId);
        if (null == response)
        {
            return null;
        }
        return DeepClone(response);
    }
    
    public IUserRepository GetUserRepository()
    {
        return new UserRepository(_inMemoryStorage.Value);
    }
    public IFormRepository GetFormRepository()
    {
        return new FormRepository(_inMemoryStorage.Value);
    }
    
    public IQuestionRepository GetQuestionRepository()
    {
        return new QuestionRepository(_inMemoryStorage.Value);
    }

    public IResponseRepository GetResponseRepository()
    {
        return new ResponseRepository(_inMemoryStorage.Value);
    }


    public IList<Guid> QuestionGuids { get; } = new List<Guid>
    {
        new("23b19020-8709-1010-a200-11397aa416dc"),
        new("23b3001e-4a2e-4010-33f0-2243aaf59238")
    };
    
    public IList<Guid> FormGuids { get; } = new List<Guid>
    {
        new("001000cd-44f4-4f44-aabb-3d96cc2cbf2e"),
    };

    public IList<Guid> UserGuids { get; } = new List<Guid>
    {
        new("99199199-3223-4ff4-aabb-3333cc2cbf2e")
    };

    public IList<Guid> ResponseGuids { get; } = new List<Guid>
    {
        new("7829abef-8c12-4bd4-93f3-75c8e1d7a0aa"),
        new("35d74f7c-85f1-42a9-9e47-1d91cbe3f22b"),
        new("62f3e1bc-5f98-48b9-8c5e-4fbe5a3e299a"),
        new("a9b5dc6e-2e37-44d3-a3bf-6f5b8d1f4e1c")
    };
    
    private Storage CreateInMemoryStorage()
    {
        var storage = new Storage(false);
        SeedStorage(storage);
        return storage;
    }
    
 private void SeedStorage(Storage storage)
{
    // Krok 1: Přidání uživatelů bez závislostí na FormEntity nebo QuestionEntity
    storage.Users.Add(new UserEntity
    {
        Id = UserGuids[0],
        FirstName = "John",
        LastName = "Doe",
        Email = "john.doe@example.com",
        PasswordHash = "hashedPassword123",
        PhotoUrl = "https://i.ibb.co/ZdZ7rK8/user-1.jpg",
        Forms = new List<FormEntity>() // Inicializováno jako prázdný seznam, přidá se později
    });

    storage.Users.Add(new UserEntity
    {
        Id = UserGuids[1],
        FirstName = "Jane",
        LastName = "Smith",
        Email = "jane.smith@example.com",
        PasswordHash = "hashedPassword456",
        PhotoUrl = "https://i.ibb.co/ZdZ7rK8/user-2.jpg",
        Forms = new List<FormEntity>()
    });

    // Krok 2: Přidání formulářů bez vazeb na Questions a User
    storage.Forms.Add(new FormEntity
    {
        Id = FormGuids[0],
        Name = "VUT FIT",
        Description = "Bud FIT",
        DateOpen = DateTime.Now.AddDays(-5),
        DateClose = DateTime.Now.AddDays(30),
        UserId = UserGuids[0],
        Questions = new List<QuestionEntity>() // Prázdný seznam, doplní se později
    });

    storage.Forms.Add(new FormEntity
    {
        Id = FormGuids[1],
        Name = "Novy formular",
        Description = "Formular pro testovani",
        DateOpen = DateTime.Now.AddDays(-3),
        DateClose = DateTime.Now.AddDays(30),
        UserId = UserGuids[0],
        Questions = new List<QuestionEntity>()
    });

    // Krok 3: Přidání otázek bez vazby na FormEntity
    storage.Questions.Add(new QuestionEntity
    {
        Id = QuestionGuids[0],
        Name = "Otazka cislo jedna",
        Description = "Napis",
        QuestionType = QuestionType.OpenQuestion,
        FormId = FormGuids[0]
    });

    storage.Questions.Add(new QuestionEntity
    {
        Id = QuestionGuids[1],
        Name = "Otazka cislo dva",
        Description = "Napis 2",
        QuestionType = QuestionType.Selection,
        FormId = FormGuids[0]
    });

    // Krok 4: Nastavení vazeb mezi Questions a Forms
    storage.Forms[0].Questions.Add(storage.Questions.First(q => q.Id == QuestionGuids[0]));
    storage.Forms[0].Questions.Add(storage.Questions.First(q => q.Id == QuestionGuids[1]));
    
    // Krok 5: Nastavení vazeb mezi Forms a Users
    storage.Users[0].Forms.Add(storage.Forms.First(f => f.Id == FormGuids[0]));
    
    // Krok 6: Přidání odpovědí (Responses) na otázky od dvou uživatelů
    storage.Responses.Add(new ResponseEntity
    {
        Id = ResponseGuids[0],
        UserId = UserGuids[0],
        QuestionId = QuestionGuids[0],
        User = storage.Users[0],
        Question = storage.Questions[0],
        UserResponse = new List<string> { "Odpoved od Johna na Otazku 1" }
    });

    storage.Responses.Add(new ResponseEntity
    {
        Id = ResponseGuids[1],
        UserId = UserGuids[1],
        QuestionId = QuestionGuids[0],
        User = storage.Users[1],
        Question = storage.Questions[0],
        UserResponse = new List<string> { "Odpoved od Jane na Otazku 1" }
    });

    storage.Responses.Add(new ResponseEntity
    {
        Id = ResponseGuids[2],
        UserId = UserGuids[0],
        QuestionId = QuestionGuids[1],
        User = storage.Users[0],
        Question = storage.Questions[1],
        UserResponse = new List<string> { "Ano od Johna" }
    });

    storage.Responses.Add(new ResponseEntity
    {
        Id = ResponseGuids[3],
        UserId = UserGuids[1],
        QuestionId = QuestionGuids[1],
        User = storage.Users[1],
        Question = storage.Questions[1],
        UserResponse = new List<string> { "Nie od Jane" }
    });
    
    // Nastavení vazeb mezi Responses a Questions
        storage.Questions[0].Responses.Add(storage.Responses.First(r => r.Id == ResponseGuids[0]));
        storage.Questions[0].Responses.Add(storage.Responses.First(r => r.Id == ResponseGuids[1]));
        storage.Questions[1].Responses.Add(storage.Responses.First(r => r.Id == ResponseGuids[2]));
        storage.Questions[1].Responses.Add(storage.Responses.First(r => r.Id == ResponseGuids[3]));

    // Nastavení vazeb mezi Responses a Users
        storage.Users[0].Responses.Add(storage.Responses.First(r => r.Id == ResponseGuids[0]));
        storage.Users[0].Responses.Add(storage.Responses.First(r => r.Id == ResponseGuids[2]));
        storage.Users[1].Responses.Add(storage.Responses.First(r => r.Id == ResponseGuids[1]));
        storage.Users[1].Responses.Add(storage.Responses.First(r => r.Id == ResponseGuids[3]));
}

}