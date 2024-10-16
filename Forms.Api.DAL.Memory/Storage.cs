using Forms.Api.DAL.Common.Entities;
using Forms.Common.Enums;

namespace Forms.Api.DAL.Memory;

   public class Storage
    {
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
        
        private readonly IList<Guid> _responseGuids = new List<Guid>
        {
            new("a62a2fb6-2b80-45b1-8f82-1401a6834abe"),
            new("78c2a34b-1e84-40c8-bc59-49510478679d"),
            new("fabde0cd-eefe-443f-baf6-3d96cc2cbf2e"),
            new("a8ee7ce8-9903-4f42-afb4-b2c34dfb7ccf"),
            new("c3542130-589c-4302-a441-a110fcadd45a"),
        };
        
        public IList<UserEntity> Users { get; } = new List<UserEntity>(); 
        public IList<QuestionEntity> Questions { get; } = new List<QuestionEntity>();
        public IList<FormEntity> Forms { get; } = new List<FormEntity>();
        public IList<ResponseEntity> Responses { get; } = new List<ResponseEntity>();


        public Storage(bool seedData = true)
        {
            if (seedData)
            {
                SeedUsers();
                SeedQuestions();
                SeedForms();
                SeedResponses();
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
                Form = Forms[0],
                Answer = new List<string>()
            });
            
            Questions.Add(new QuestionEntity
            {
                Id = _questionGuids[1],
                Name = "Question 2",
                Description = "Vyber jednu z moznosti",
                QuestionType = QuestionType.Selection,
                FormId = _formGuids[0],
                Form = Forms[0],
                Answer = new List<string>{"Som muž", "Som žena" }
            });
            
            Questions.Add(new QuestionEntity
            {
                Id = _questionGuids[2],
                Name = "Question 3",
                Description = "Vyber z idealniho mista dovolene :D",
                QuestionType = QuestionType.Selection,
                FormId = _formGuids[2],
                Form = Forms[2],
                Answer = new List<string>{"Grécko", "Taliansko", "Egypt", "Bulharsko"}
            });
            
            Questions.Add(new QuestionEntity
            {
                Id = _questionGuids[3],
                Name = "Question 4",
                Description = "Kolik jsi mel na bodu z ISA cvika?",
                QuestionType = QuestionType.Range,
                FormId = _formGuids[1],
                Form = Forms[1],
                Answer = new List<string>{}   // todo doplnit range odpovedi
            });
        }
        
        private void SeedForms()
        {
            Forms.Add(new FormEntity
            {
                Id = _formGuids[0],
                Name = "VUT FIT",
                Description = "Bud FIT",
                DateOpen = DateTime.Now.AddDays(-5),            // Five Days Ago
                DateClose = DateTime.Now.AddDays(30),
                UserId = _userGuids[0],
                User = Users[0],                                // John Doe
                Questions = Questions.Where(q => q.FormId == _formGuids[0]).ToList()
            });
            
            Forms.Add(new FormEntity
            {
                Id = _formGuids[1],
                Name = "VUT FIT",
                Description = "Bud FIT",
                DateOpen = DateTime.Now.AddDays(-5),            // Five Days Ago
                DateClose = DateTime.Now.AddDays(30),
                UserId = _userGuids[1],
                User = Users[1],                                // Jane Doe
                Questions = Questions.Where(q => q.FormId == _formGuids[1]).ToList()
            });
            
            Forms.Add(new FormEntity
            {
                Id = _formGuids[2],
                Name = "Dovolena Form",
                Description = "Formular pro vyber dovolene",
                DateOpen = DateTime.Now.AddDays(-5),            // Five Days Ago
                DateClose = DateTime.Now.AddDays(30),
                UserId = _userGuids[2],
                User = Users[2],                                // Alice Wonderland
                Questions = Questions.Where(q => q.FormId == _formGuids[2]).ToList()
            });
        }
        
        
        private void SeedResponses()
        {
            Responses.Add(new ResponseEntity
            {
                Id = _responseGuids[0],
                UserId = _userGuids[0],
                QuestionId = _questionGuids[0],
                User = Users[0],
                Question = Questions[0],
                UserResponse = new List<string>{"Napíšem ti Hello World!!!"}
            });
            
            Responses.Add(new ResponseEntity
            {
                Id = _responseGuids[1],
                UserId = _userGuids[0],
                QuestionId = _questionGuids[1],
                User = Users[0],
                Question = Questions[1],
                UserResponse = new List<string>{Questions[1].Answer[1]}   // "Som žena"
            });
            
            Responses.Add(new ResponseEntity
            {
                Id = _responseGuids[2],
                UserId = _userGuids[2],
                QuestionId = _questionGuids[2],
                User = Users[2],
                Question = Questions[2],
                UserResponse = new List<string>{Questions[2].Answer[0], Questions[2].Answer[2]}   // "Grécko", "Egypt"
            });
            
            // todo range odpoved dopisat
            Responses.Add(new ResponseEntity
            {
                Id = _responseGuids[3],
                UserId = _userGuids[1],
                QuestionId = _questionGuids[3],
                User = Users[1],
                Question = Questions[3],
                UserResponse = new List<string>{}   // odpovede
            });
            
        }
    }