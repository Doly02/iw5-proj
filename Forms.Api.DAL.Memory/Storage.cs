using Forms.Api.DAL.Common.Entities;
using Forms.Common.Enums;

namespace Forms.Api.DAL.Memory;

   public class Storage
    {
        // todo guids
        private readonly IList<Guid> _userGuids = new List<Guid>
        {
            new("a23b7c9f-3edb-45ef-8a5b-3e7a2b5d9289"),
            new("c4d5e28b-becd-41f7-8d6e-df93ff454321"),
            new("b3f62b8b-291c-42e5-b8d9-3afbb3276dca")
        };

        private readonly IList<Guid> _questionGuids = new List<Guid>
        {
            new("fffb7c9f-3edb-45ef-aa5b-3e2a2b5d9289"),
            new("c1dff28b-b11d-4115-8888-ba93ff454321"),
            new("b8f99b8b-c91c-4ee5-8899-3afbb3276dca"),
            new("aad1223b-b11d-4115-5588-aa93ff456621"),
            new("185469bb-a449-3335-2399-1af3ff276dca")
        };

        private readonly IList<Guid> _formGuids = new List<Guid>
        {
            new("b8f6666b-c91c-4ee5-8899-3afbb3276dca"),
            new("b8f1111b-c33c-4005-8899-3afbb3276dca"),
            new("b8f2222b-c22c-4115-8899-3afbb3276dca"),
            new("b8f3333b-c11c-4225-8899-3afbb3276dca")
        };
        // todo lists of entities
        public IList<UserEntity> Users { get; } = new List<UserEntity>();

        public IList<FormEntity> Forms { get; } = new List<FormEntity>();
        public IList<QuestionEntity> Questions { get; } = new List<QuestionEntity>();
        public IList<ResponseEntity> Responses { get; } = new List<ResponseEntity>();

        public Storage(bool seedData = true)
        {
            if (seedData)
            {
                SeedUsers();
                SeedQuestions();
            }
        }
        
        private void SeedUsers()
        {
            Users.Add(new UserEntity
            {
                Id = _userGuids[0],
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PasswordHash = "hashedPassword123",
                PhotoUrl = "https://i.ibb.co/ZdZ7rK8/user-1.jpg"
            });

            Users.Add(new UserEntity
            {
                Id = _userGuids[1],
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com",
                PasswordHash = "hashedPassword456",
                PhotoUrl = "https://i.ibb.co/ZdZ7rK8/user-2.jpg"
            });

            Users.Add(new UserEntity
            {
                Id = _userGuids[2],
                FirstName = "Alice",
                LastName = "Wonderland",
                Email = "alice.wonderland@example.com",
                PasswordHash = "hashedPassword789",
                PhotoUrl = "https://i.ibb.co/ZdZ7rK8/user-3.jpg"
            });
        }

        private void SeedQuestions()
        {
            Questions.Add(new QuestionEntity
            {
                Id = _questionGuids[0],
                Name = "Question 1",
                Description = "Napis",
                QuestionType = QuestionType.OpenQuestion,
                FormId = _formGuids[0],
                Form = new FormEntity
                {
                    Id = _formGuids[0],
                    Name = "VUT FIT",
                    Description = "Bud FIT",
                    DateOpen = DateTime.Now.AddDays(-5),            // Five Days Ago
                    DateClose = DateTime.Now.AddDays(30),
                    UserId = _userGuids[0]
                }
            });
            
            Questions.Add(new QuestionEntity
            {
                Id = _questionGuids[1],
                Name = "Question 2",
                Description = "Vyber jednu z moznosti",
                QuestionType = QuestionType.Selection,
                FormId = _formGuids[0],
                Form = new FormEntity
                {
                    Id = _formGuids[0],
                    Name = "VUT FIT",
                    Description = "Bud FIT",
                    DateOpen = DateTime.Now.AddDays(-5),            // Five Days Ago
                    DateClose = DateTime.Now.AddDays(30),
                    UserId = _userGuids[0]
                }
            });
            
            Questions.Add(new QuestionEntity
            {
                Id = _questionGuids[2],
                Name = "Question 3",
                Description = "Vyber z idealniho mista dovolene :D",
                QuestionType = QuestionType.Selection,
                FormId = _formGuids[2],
                Form = new FormEntity
                {
                    Id = _formGuids[2],
                    Name = "Dovolena Form",
                    Description = "Formular pro vyber dovolene",
                    DateOpen = DateTime.Now.AddDays(-5),            // Five Days Ago
                    DateClose = DateTime.Now.AddDays(30),
                    UserId = _userGuids[2]
                }
            });
            
            Questions.Add(new QuestionEntity
            {
                Id = _questionGuids[3],
                Name = "Question 4",
                Description = "Kolik jsi mel na bodu z ISA cvika?",
                QuestionType = QuestionType.Range,
                FormId = _formGuids[1],
                Form = new FormEntity
                {
                    Id = _formGuids[1],
                    Name = "VUT FIT",
                    Description = "Bud FIT",
                    DateOpen = DateTime.Now.AddDays(-5),            // Five Days Ago
                    DateClose = DateTime.Now.AddDays(30),
                    UserId = _userGuids[1]
                }
            });
        }
    }