using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;

namespace Forms.Api.DAL.Memory.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IList<UserEntity> _users;
        private readonly IList<QuestionEntity> _questions;
        private readonly IList<ResponseEntity> _responses;

        public UserRepository(
            Storage storage)
        {
            this._users = storage.Users;
            this._questions = storage.Questions;
            this._responses = storage.Responses;

        }
        
        public IList<UserEntity> GetAll()
        {
            return this._users;
        }

        public UserEntity? GetById(Guid id)
        {
            var userEntity = _users.SingleOrDefault(user => user.Id == id);
            return userEntity;
        }

        public Guid Insert(UserEntity entity)
        {
            _users.Add(entity);
            return entity.Id;
        }

        public Guid? Update(UserEntity entity)
        {
            var userEntityExisting = _users.SingleOrDefault(userEntity => userEntity.Id == entity.Id);

            if (userEntityExisting is not null)
            {
                // userEntityExisting.Responses = GetResponsesByUserId(entity.Id);
                // UpdateUser(entity, userEntityExisting);
                return userEntityExisting.Id;
            }
            else
            {
                return null;
            }
        }

        public void Remove(Guid id)
        {
            var userToRemove = _users.Where(user => user.Id == id).ToList();

            for (var i = 0; i < userToRemove.Count; i++)
            {
                var userBeingRemoved = userToRemove.ElementAt(i);
                _users.Remove(userBeingRemoved);
            }
            
        }

        public bool Exists(Guid id)
        {
            return _users.Any(user => user.Id == id);
        }
    }
    
}