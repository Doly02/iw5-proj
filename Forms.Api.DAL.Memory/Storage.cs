using Forms.Api.DAL.Common.Entities;

namespace Forms.Api.DAL.Memory;

   public class Storage
    {
        // todo guids
        private readonly IList<Guid> userGuids = new List<Guid>
        {
            new("a23b7c9f-3edb-45ef-8a5b-3e7a2b5d9289"),
            new("c4d5e28b-becd-41f7-8d6e-df93ff454321"),
            new("b3f62b8b-291c-42e5-b8d9-3afbb3276dca")
        };

        
        // todo lists of entities
        public IList<UserEntity> Users { get; } = new List<UserEntity>();

        public Storage(bool seedData = true)
        {
            if (seedData)
            {
                // SeedUsers(); etc.
                SeedUsers();
            }
        }
        
        private void SeedUsers()
        {
            Users.Add(new UserEntity
            {
                Id = userGuids[0],
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PasswordHash = "hashedPassword123",
                PhotoUrl = "https://i.ibb.co/ZdZ7rK8/user-1.jpg"
            });

            Users.Add(new UserEntity
            {
                Id = userGuids[1],
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com",
                PasswordHash = "hashedPassword456",
                PhotoUrl = "https://i.ibb.co/ZdZ7rK8/user-2.jpg"
            });

            Users.Add(new UserEntity
            {
                Id = userGuids[2],
                FirstName = "Alice",
                LastName = "Wonderland",
                Email = "alice.wonderland@example.com",
                PasswordHash = "hashedPassword789",
                PhotoUrl = "https://i.ibb.co/ZdZ7rK8/user-3.jpg"
            });
        }

        
    
    }