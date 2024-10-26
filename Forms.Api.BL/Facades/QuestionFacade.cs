using System;
using System.Collections.Generic;
using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;
using Forms.Common.Models.Question;
using Forms.Common.Models.Search;

namespace Forms.Api.BL.Facades
{
    public class QuestionFacade : IQuestionFacade
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IMapper _mapper;

        public QuestionFacade(
            IQuestionRepository questionRepository,
            IMapper mapper)
        {
            this._questionRepository = questionRepository;
            this._mapper = mapper;
        }

        public List<QuestionListModel> GetAll()
        {
            return _mapper.Map<List<QuestionListModel>>(_questionRepository.GetAll());
        }

        public QuestionDetailModel? GetById(Guid id)
        {
            var questionEntity = _questionRepository.GetById(id);
            return _mapper.Map<QuestionDetailModel>(questionEntity);
        }

        public Guid CreateOrUpdate(QuestionDetailModel questionModel)
        {
            return _questionRepository.Exists(questionModel.Id)
                ? Update(questionModel)!.Value
                : Create(questionModel);
        }

        public Guid Create(QuestionDetailModel questionModel)
        {
            var questionEntity = _mapper.Map<QuestionEntity>(questionModel);
            return _questionRepository.Insert(questionEntity);
        }

        public Guid? Update(QuestionDetailModel questionModel)
        {
            var questionEntity = _mapper.Map<QuestionEntity>(questionModel);
            return _questionRepository.Update(questionEntity);
        }

        public void Delete(Guid id)
        {
            _questionRepository.Remove(id);
        }
        
        public async Task<List<SearchResultModel>> SearchAsync(string query)
        {
            var questions = await _questionRepository.SearchAsync(query);
            return _mapper.Map<List<SearchResultModel>>(questions);
        }
    }
}