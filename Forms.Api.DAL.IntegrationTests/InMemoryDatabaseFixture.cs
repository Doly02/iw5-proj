using System;
using System.Collections.Generic;
using System.Linq;
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
    
    public QuestionEntity? GetQuestionDirect(Guid questionId)
    {
        var question = _inMemoryStorage.Value.Questions.SingleOrDefault(t => t.Id == questionId);
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
    public IUserRepository GetUserRepository()
    {
        return new UserRepository(_inMemoryStorage.Value);
    }
    public IList<Guid> QuestionGuids { get; } = new List<Guid>
    {
        new("23b19020-8709-1010-a200-11397aa416dc"),
        new("23b3001e-4a2e-4010-33f0-2243aaf59238")
    };
    
    public IList<Guid> FormGuids { get; } = new List<Guid>
    {
        new("001000cd-44f4-4f44-aabb-3d96cc2cbf2e")
    };

    public IList<Guid> UserGuids { get; } = new List<Guid>
    {
        new("99199199-3223-4ff4-aabb-3333cc2cbf2e")
    };
    private Storage CreateInMemoryStorage()
    {
        var storage = new Storage(true);
        SeedStorage(storage);
        return storage;
    }
    
    private void SeedStorage(Storage storage)
    {
        storage.Questions.Add(new QuestionEntity
        {
            Id = QuestionGuids[0],
            Name = "Otazka cislo jedna",
            Description = "Napis",
            QuestionType = QuestionType.OpenQuestion,
            FormId = FormGuids[0],
            Form = new FormEntity
            {
                Id = FormGuids[0],
                Name = "VUT FIT",
                Description = "Bud FIT",
                DateOpen = DateTime.Now.AddDays(-5),                // Five Days Ago
                DateClose = DateTime.Now.AddDays(30),
                UserId = UserGuids[0],
                User = storage.Users[0],                            // John Doe
            }
        });
        
        storage.Questions.Add(new QuestionEntity
        {
            Id = QuestionGuids[1],
            Name = "Otazka cislo dva",
            Description = "Napis 2",
            QuestionType = QuestionType.OpenQuestion,
            FormId = FormGuids[0],
            Form = new FormEntity
            {
                Id = FormGuids[0],
                Name = "VUT FIT",
                Description = "Bud FIT",
                DateOpen = DateTime.Now.AddDays(-5),                // Five Days Ago
                DateClose = DateTime.Now.AddDays(30),
                UserId = UserGuids[0],
                User = storage.Users[0],                            // John Doe
            }
        });
        
        storage.Users.Add(new UserEntity
        {
            Id = UserGuids[0],
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PasswordHash = "hashedPassword123",
            PhotoUrl = "https://i.ibb.co/ZdZ7rK8/user-1.jpg",
            Forms = new List<FormEntity> { storage.Forms.First(f => f.Id == FormGuids[0]) }
        });

        storage.Forms.Add(new FormEntity
        {
            Id = FormGuids[0],
            Name = "VUT FIT",
            Description = "Bud FIT",
            DateOpen = DateTime.Now.AddDays(-5),                // Five Days Ago
            DateClose = DateTime.Now.AddDays(30),
            UserId = UserGuids[0],
            User = storage.Users[0],                            // John Doe
            Questions = new List<QuestionEntity>    
            {
                storage.Questions.First(q => q.Id == QuestionGuids[0]),
                storage.Questions.First(q => q.Id == QuestionGuids[1])
            }
        });
        
        storage.Forms.Add(new FormEntity
        {
            Id = FormGuids[1],
            Name = "Novy formular",
            Description = "Formular pro testovani",
            DateOpen = DateTime.Now.AddDays(-3),                // Opened 3 Days Ago
            DateClose = DateTime.Now.AddDays(30),
            UserId = UserGuids[1],
            User = storage.Users[1]
        });
    }
}