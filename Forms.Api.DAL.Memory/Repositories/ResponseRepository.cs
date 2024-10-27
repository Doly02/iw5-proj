using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;

namespace Forms.Api.DAL.Memory.Repositories
{
    public class ResponseRepository(Storage storage) : IResponseRepository
    {
        private readonly IList<ResponseEntity> _responses = storage.Responses;

        public IList<ResponseEntity> GetAll()
        {
            return _responses;
        }

        public ResponseEntity? GetById(Guid id)
        {
            return _responses.SingleOrDefault(entity => entity.Id == id);
        }

        public Guid Insert(ResponseEntity response)
        {
            _responses.Add(response);
            return response.Id;
        }

        public Guid? Update(ResponseEntity entity)
        {
            var responseEntityExisting = _responses.SingleOrDefault(r => r.Id == entity.Id);
            if (responseEntityExisting is null) return null;

            responseEntityExisting.UserId = entity.UserId;
            responseEntityExisting.QuestionId = entity.QuestionId;
            responseEntityExisting.User = entity.User;
            responseEntityExisting.Question = entity.Question;
            responseEntityExisting.UserResponse = entity.UserResponse;

            return responseEntityExisting.Id;
        }

        public void Remove(Guid id)
        {
            var responseToRemove = _responses.SingleOrDefault(r => r.Id == id);
            if (responseToRemove != null)
            {
                _responses.Remove(responseToRemove);
            }
        }

        public bool Exists(Guid id)
        {
            return _responses.Any(response => response.Id == id);
        }
    }
}