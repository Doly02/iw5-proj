using Forms.Api.DAL.Common.Entities;

namespace Forms.Api.DAL.Memory;

   public class Storage
    {
        private readonly IList<Guid> FormGuids = new List<Guid>
        {
            new("df935095-8709-4040-a2bb-b6f97cb416dc"),
            new("23b3902d-7d4f-4213-9cf0-112348f56238"),
            new("7f251cd6-3ac4-49be-b3e7-d1f9f7cfdd3a"),
        };

        private readonly IList<Guid> QuestionGuids = new List<Guid>
        {
            new("0d4fa150-ad80-4d46-a511-4c666166ec5e"),
            new("87833e66-05ba-4d6b-900b-fe5ace88dbd8"),
            new("adb7daf1-8a6c-4cbb-b4f5-631a9b7f7287"),
            new("a8978e5d-0c5b-449c-9dc0-0a01563c0c3b"),
            new("0e88301e-cd92-47cf-8ee7-5cb0752e9f82"),
            new("e79f129f-3153-41df-8e84-8bcd7a077648"),
        };

        private readonly IList<Guid> UserGuids = new List<Guid>
        {
            new("fabde0cd-eefe-443f-baf6-3d96cc2cbf2e"),
            new("a8ee7ce8-9903-4f42-afb4-b2c34dfb7ccf"),
            new("c3542130-589c-4302-a441-a110fcadd45a"),
        };
        
        private readonly IList<Guid> ResponseGuids = new List<Guid>
        {
            new("a62a2fb6-2b80-45b1-8f82-1401a6834abe"),
            new("78c2a34b-1e84-40c8-bc59-49510478679d"),
        };

        public IList<FormEntity> Forms { get; } = new List<FormEntity>();
        public IList<QuestionEntity> Questions { get; } = new List<QuestionEntity>();
        public IList<UserEntity> Users { get; } = new List<UserEntity>();
        public IList<ResponseEntity> Responses { get; } = new List<ResponseEntity>();


        public Storage(bool seedData = true)
        {
            if (seedData)
            {   
                SeedForms();
                SeedQuestions();
                SeedUsers();
                SeedResponses();
            }
        }

        // TODO add datas
        private void SeedForms()
        {
            
        }

        private void SeedQuestions()
        {
            
        }

        private void SeedUsers()
        {
            
        }
        
        private void SeedResponses()
        {
            
        }
    }