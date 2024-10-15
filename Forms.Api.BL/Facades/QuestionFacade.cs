using System;
using System.Collections.Generic;
using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;
using Forms.Common.Models.Question;

namespace Forms.Api.BL.Facades
{
    public class QuestionFacade : IQuestionFacade
    {
        private readonly IQuestionRepository questionRepository;
        private readonly IMapper mapper;

        public QuestionFacade(
            IQuestionRepository questionRepository,
            IMapper mapper)
        {
            this.questionRepository = questionRepository;
            this.mapper = mapper;
        }

        public List<QuestionListModel> GetAll()
        {
            return mapper.Map<List<QuestionListModel>>(questionRepository.GetAll());
        }

        public QuestionDetailModel? GetById(Guid id)
        {
            var questionEntity = questionRepository.GetById(id);
            return mapper.Map<QuestionDetailModel>(questionEntity);
        }

        public Guid CreateOrUpdate(QuestionDetailModel questionModel)
        {
            return questionRepository.Exists(questionModel.Id)
                ? Update(questionModel)!.Value
                : Create(questionModel);
        }

        public Guid Create(QuestionDetailModel questionModel)
        {
            var questionEntity = mapper.Map<QuestionEntity>(questionModel);
            return questionRepository.Insert(questionEntity);
        }

        public Guid? Update(QuestionDetailModel questionModel)
        {
            var questionEntity = mapper.Map<QuestionEntity>(questionModel);
            return questionRepository.Update(questionEntity);
        }

        public void Delete(Guid id)
        {
            questionRepository.Remove(id);
        }
    }
}