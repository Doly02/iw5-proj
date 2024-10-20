using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;

namespace Forms.Api.DAL.Memory.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly IList<QuestionEntity> _questions;
        private readonly IMapper _mapper;

        public QuestionRepository(
            Storage storage,
            IMapper mapper)
        {
            _questions = storage.Questions;
            _mapper = mapper;
        }

        public IList<QuestionEntity> GetAll()
        {
            return _questions;
        }

        public QuestionEntity? GetById(Guid id)
        {
            return _questions.SingleOrDefault(entity => entity.Id == id);
        }

        public Guid Insert(QuestionEntity question)
        {
            _questions.Add(question);
            return question.Id;
        }

        public Guid? Update(QuestionEntity entity)
        {
            var questionExists = _questions.SingleOrDefault(questionInStore =>
                questionInStore.Id == entity.Id);
            if (null != questionExists) {
                _mapper.Map(entity, questionExists);
            }
            return questionExists?.Id;
        }

        public void Remove(Guid id)
        {
            var questionToRemove = _questions.Single(question => question.Id.Equals(id));
            _questions.Remove(questionToRemove);
        }

        public bool Exists(Guid id)
        {
            return _questions.Any(question => question.Id == id);
        }
    }
}
