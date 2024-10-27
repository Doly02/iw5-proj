using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;

namespace Forms.Api.DAL.Memory.Repositories
{
    public class QuestionRepository(Storage storage) : IQuestionRepository
    {
        private readonly IList<QuestionEntity> _questions = storage.Questions;
        private IList<ResponseEntity> _responses = storage.Responses;

        public IList<QuestionEntity> GetAll()
        {
            return _questions;
        }

        public QuestionEntity? GetById(Guid id)
        {
            return _questions.SingleOrDefault(q => q.Id == id);
        }

        public Guid Insert(QuestionEntity entity)
        {
            _questions.Add(entity);
            return entity.Id;
        }

        public Guid? Update(QuestionEntity entity)
        {
            var questionEntityExisting = _questions.SingleOrDefault(q => q.Id == entity.Id);
            if (questionEntityExisting is null) return null;
            
            questionEntityExisting.Name = entity.Name;
            questionEntityExisting.Description = entity.Description;
            questionEntityExisting.QuestionType = entity.QuestionType;
            questionEntityExisting.Answer = entity.Answer;
            questionEntityExisting.FormId = entity.FormId;
            
            questionEntityExisting.Responses = GetResponsesByQuestionId(entity.Id);

            return questionEntityExisting.Id;
        }

        public void AddResponse(Guid questionId, ResponseEntity response)
        {
            var question = _questions.SingleOrDefault(q => q.Id == questionId);
            if (question != null)  _responses.Add(response);
        }
        
        public void UpdateResponse(Guid responseId, ResponseEntity updatedResponse)
        {
            var response = _responses.SingleOrDefault(r => r.Id == responseId);
            if (response == null) return;
            
            response.UserId = updatedResponse.UserId;
            response.QuestionId = updatedResponse.QuestionId;
            response.User = updatedResponse.User;
            response.Question = updatedResponse.Question;
            response.UserResponse = updatedResponse.UserResponse;
        }

        public void DeleteResponse(Guid responseId)
        {
            var response = _responses.SingleOrDefault(r => r.Id == responseId);
            if (response != null)  _responses.Remove(response);
        }
        
        private IList<ResponseEntity> GetResponsesByQuestionId(Guid questionId)
        {
            return _responses.Where(response => response.QuestionId == questionId).ToList();
        }

        public void Remove(Guid id)
        {
            var questionToRemove = _questions.SingleOrDefault(q => q.Id == id);
            if (questionToRemove == null) return;
            
            var responsesToRemove = _responses.Where(response => response.QuestionId == id).ToList();
            foreach (var response in responsesToRemove)
            {
                _responses.Remove(response);
            }

            _questions.Remove(questionToRemove);
        }


        public bool Exists(Guid id)
        {
            return _questions.Any(q => q.Id == id);
        }
        
        public Task<List<QuestionEntity>> SearchAsync(string query)
        {
            query = query.ToLower(); 

            var results = _questions
                .Where(q => 
                    q.Name.ToLower().Contains(query) || 
                    q.Description.ToLower().Contains(query))
                .ToList();

            return Task.FromResult(results);
        }
    }
}
