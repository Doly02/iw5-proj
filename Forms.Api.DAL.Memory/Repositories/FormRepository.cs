using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;

namespace Forms.Api.DAL.Memory.Repositories
{
    public class FormRepository : IFormRepository
    {
        private IList<QuestionEntity> questions;
        private IList<FormEntity> forms;
        private IList<UserEntity> users;
        
        public FormRepository(
            Storage storage)
        {
            this.forms = storage.Forms;
            this.questions = storage.Questions;
            this.users = storage.Users;
        }
        
        public IList<FormEntity> GetAll()
        {
            return this.forms;
        }

        public FormEntity? GetById(Guid id)
        {
            var formEntity = forms.SingleOrDefault(form => form.Id == id);
            return formEntity;
        }

        public Guid Insert(FormEntity entity)
        {
            forms.Add(entity);
            return entity.Id;
        }

        public Guid? Update(FormEntity entity)
        {
            var formEntityExisting = forms.SingleOrDefault(form => form.Id == entity.Id);
            if (formEntityExisting is null) return null;

            /* Update Attributes of The Form */
            formEntityExisting.Name = entity.Name;
            formEntityExisting.Description = entity.Description;
            formEntityExisting.DateOpen = entity.DateOpen;
            formEntityExisting.DateClose = entity.DateClose;
            formEntityExisting.UserId = entity.UserId;

            UpdateQuestions(formEntityExisting, entity.Questions);

            return formEntityExisting.Id;
        }
        
        private IList<QuestionEntity> GetQuestionByFormId(Guid formId)
        {
            return questions.Where(question => question.FormId == formId).ToList();
        }

        private void UpdateQuestions(FormEntity existingForm, ICollection<QuestionEntity> updatedQuestions)
        {
            /* Clear Current Questions And Add New */
            existingForm.Questions.Clear();
            foreach (var question in updatedQuestions)
            {
                existingForm.Questions.Add(question);
            }
        }
        
        private void DeleteResponses(FormEntity formEntity, IEnumerable<FormEntity> formToDelete)
        {
            foreach (var response in formToDelete)
            {
                forms.Remove(response);
            }
        }


        private void InsertQuestions(FormEntity formEntity, IEnumerable<QuestionEntity> questionToInsert)
        {
            foreach (var question in questionToInsert)
            {
                questions.Add(question);
            }
        }

        private void ModifyQuestions(UserEntity formEntity, IEnumerable<QuestionEntity> questionToUpdate)
        {
            foreach (var updatedQuestion in questionToUpdate)
            {
                var existingQuestion = questions.SingleOrDefault(question => question.Id == updatedQuestion.Id);
                if (existingQuestion != null)
                {
                    existingQuestion.Name = updatedQuestion.Name;
                    existingQuestion.Description = updatedQuestion.Description;
                    existingQuestion.QuestionType = updatedQuestion.QuestionType;
                }
            }
        }

        private void UpdateQuestions(FormEntity formEntity, IEnumerable<QuestionEntity> updatedQuestions)
        {
            /* Delete Question That Are Not Already In The Form */
            var questionsToDelete = formEntity.Questions
                .Where(existingQuestion => updatedQuestions.All(uq => uq.Id != existingQuestion.Id))
                .ToList();
            foreach (var question in questionsToDelete)
            {
                formEntity.Questions.Remove(question);
            }
            
            /* Add New Question, That Are Not Yet In The Form */
            var questionsToAdd = updatedQuestions
                .Where(newQuestion => formEntity.Questions.All(existingQuestion => existingQuestion.Id != newQuestion.Id))
                .ToList();
            foreach (var question in questionsToAdd)
            {
                formEntity.Questions.Add(question);
            }

            /* Update Existing Questions, If They Are Changed */
            var questionsToUpdate = updatedQuestions
                .Where(newQuestion => formEntity.Questions.Any(existingQuestion => existingQuestion.Id == newQuestion.Id))
                .ToList();
            foreach (var updatedQuestion in questionsToUpdate)
            {
                var existingQuestion = formEntity.Questions.First(q => q.Id == updatedQuestion.Id);
                existingQuestion.Name = updatedQuestion.Name;
                existingQuestion.Description = updatedQuestion.Description;
                existingQuestion.QuestionType = updatedQuestion.QuestionType;
                existingQuestion.Answer = updatedQuestion.Answer;
                existingQuestion.Responses = updatedQuestion.Responses;
            }
        }
        private void DeleteQuestion(FormEntity formEntity, IEnumerable<QuestionEntity> questionToDelete)
        {
            var toDelete = questionToDelete.ToList();
            for (int i = 0; i < toDelete.Count; i++)
            {
                var question = toDelete.ElementAt(i);
                questions.Remove(question);
            }
        }

        private void InsertQuestion(FormEntity formEntity, IEnumerable<QuestionEntity> questionToInsert)
        {
            foreach (var question in questionToInsert)
            {
                formEntity.Questions.Add(new QuestionEntity
                {
                    Id = question.Id,
                    Name = question.Name,
                    Description = question.Description,
                    QuestionType = question.QuestionType,
                    FormId = question.FormId,
                    Form = formEntity,
                    Responses = question.Responses,
                });
            }
        }

        private void ModifyForms(UserEntity userEntity, IEnumerable<FormEntity> formsToUpdate)
        {
            foreach (var updatedForm in formsToUpdate)
            {
                var existingForm = forms.SingleOrDefault(form => form.Id == updatedForm.Id);
                if (existingForm != null)
                {
                    existingForm.Name = updatedForm.Name;
                    existingForm.Description = updatedForm.Description;
                    existingForm.DateOpen = updatedForm.DateOpen;
                    existingForm.DateClose = updatedForm.DateClose;
                }
            }
        }

            public void Remove(Guid id)
            {
                var userToRemove = users.Where(user => user.Id == id).ToList();

                for (var i = 0; i < userToRemove.Count; i++)
                {
                    var userBeingRemoved = userToRemove.ElementAt(i);
                    users.Remove(userBeingRemoved);
                }
                
            }

            public bool Exists(Guid id)
            {
                return users.Any(user => user.Id == id);
            }
        }
    
}