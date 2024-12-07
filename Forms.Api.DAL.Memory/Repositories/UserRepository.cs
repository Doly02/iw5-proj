using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;

namespace Forms.Api.DAL.Memory.Repositories
{
    public class UserRepository : IUserRepository
    {
        private IList<UserEntity> users;
        private IList<QuestionEntity> questions;
        private IList<FormEntity> forms;
        private IList<ResponseEntity> responses;

        public UserRepository(
            Storage storage)
        {
            this.users = storage.Users;
            this.forms = storage.Forms;
            this.questions = storage.Questions;
            this.responses = storage.Responses;

        }
        
        public IList<UserEntity> GetAll()
        {
            return this.users;
        }

        public UserEntity? GetById(Guid id)
        {
            var userEntity = users.SingleOrDefault(user => user.Id == id);
            return userEntity;
        }

        public Guid Insert(UserEntity entity)
        {
            users.Add(entity);
            return entity.Id;
        }

     public Guid? Update(UserEntity entity)
    {
        var userEntityExisting = users.SingleOrDefault(user => user.Id == entity.Id);
        if (userEntityExisting is null) return null;

        userEntityExisting.FirstName = entity.FirstName;
        userEntityExisting.LastName = entity.LastName;
        userEntityExisting.Email = entity.Email;
        userEntityExisting.PhotoUrl = entity.PhotoUrl;

        UpdateResponses(userEntityExisting, entity.Responses);
        UpdateForms(userEntityExisting, entity.Forms);
        userEntityExisting.Responses = GetResponsesByUserId(entity.Id);
        userEntityExisting.Forms = GetFormsByUserId(entity.Id);

        return userEntityExisting.Id;
    }
     
    private IList<ResponseEntity> GetResponsesByUserId(Guid userId)
    {
        return responses.Where(response => response.UserId == userId).ToList();
    }

    private IList<FormEntity> GetFormsByUserId(Guid userId)
    {
        return forms.Where(form => form.UserId == userId).ToList();
    }
    
    private void UpdateResponses(UserEntity userEntity, IEnumerable<ResponseEntity> updatedResponses)
    {
        var responsesToDelete = userEntity.Responses
            .Where(response => !updatedResponses.Any(ur => ur.Id == response.Id));
        DeleteResponses(userEntity, responsesToDelete);

        var responsesToInsert = updatedResponses
            .Where(ur => !userEntity.Responses.Any(response => response.Id == ur.Id));
        InsertResponses(userEntity, responsesToInsert);

        var responsesToUpdate = updatedResponses
            .Where(ur => userEntity.Responses.Any(response => response.Id == ur.Id));
        ModifyResponses(userEntity, responsesToUpdate);
    }

    private void DeleteResponses(UserEntity userEntity, IEnumerable<ResponseEntity> responsesToDelete)
    {
        foreach (var response in responsesToDelete)
        {
            responses.Remove(response);
        }
    }


    private void InsertResponses(UserEntity userEntity, IEnumerable<ResponseEntity> responsesToInsert)
    {
        foreach (var response in responsesToInsert)
        {
            responses.Add(response);
        }
    }

    private void ModifyResponses(UserEntity userEntity, IEnumerable<ResponseEntity> responsesToUpdate)
    {
        foreach (var updatedResponse in responsesToUpdate)
        {
            var existingResponse = responses.SingleOrDefault(response => response.Id == updatedResponse.Id);
            if (existingResponse != null)
            {
                existingResponse.UserResponse = updatedResponse.UserResponse;
                existingResponse.QuestionId = updatedResponse.QuestionId;
            }
        }
    }

    private void UpdateForms(UserEntity userEntity, IEnumerable<FormEntity> updatedForms)
    {
        var formsToDelete = userEntity.Forms
            .Where(form => updatedForms.All(uf => uf.Id != form.Id));
        DeleteForms(userEntity, formsToDelete);

        var formsToInsert = updatedForms
            .Where(uf => userEntity.Forms.All(form => form.Id != uf.Id));
        InsertForms(userEntity, formsToInsert);

        var formsToUpdate = updatedForms
            .Where(uf => userEntity.Forms.Any(form => form.Id == uf.Id));
        ModifyForms(userEntity, formsToUpdate);
    }

    private void DeleteForms(UserEntity userEntity, IEnumerable<FormEntity> formsToDelete)
    {
        var toDelete = formsToDelete.ToList();
        for (int i = 0; i < toDelete.Count; i++)
        {
            var form = toDelete.ElementAt(i);
            forms.Remove(form);
        }
    }

    private void InsertForms(UserEntity userEntity, IEnumerable<FormEntity> formsToInsert)
    {
        foreach (var form in formsToInsert)
        {
            forms.Add(new FormEntity
            {
                Name = form.Name,
                DateOpen = form.DateOpen,
                DateClose = form.DateClose,
                UserId = form.UserId,
                Id = form.Id,
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
        
        public async Task<List<UserEntity>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<UserEntity>();

            query = query.ToLower();

            var result = users
                .Where(user =>
                    user.FirstName.ToLower().Contains(query) ||
                    user.LastName.ToLower().Contains(query) ||
                    user.Email.ToLower().Contains(query))
                .ToList();

            return await Task.FromResult(result);
        }
    }
    
}