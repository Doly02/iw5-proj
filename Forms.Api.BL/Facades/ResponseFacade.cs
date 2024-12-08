using System;
using System.Collections.Generic;
using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;
using Forms.Common.Models.Response;

namespace Forms.Api.BL.Facades
{
    public class ResponseFacade : IResponseFacade
    {
        private readonly IResponseRepository _responseRepository;
        private readonly IMapper _mapper;

        public ResponseFacade(
            IResponseRepository responseRepository,
            IMapper mapper)
        {
            this._responseRepository = responseRepository;
            this._mapper = mapper;
        }

        public List<ResponseListModel> GetAll()
        {
            return _mapper.Map<List<ResponseListModel>>(_responseRepository.GetAll());
        }

        public ResponseDetailModel? GetById(Guid id)
        {
            var responseEntity = _responseRepository.GetById(id);
            return _mapper.Map<ResponseDetailModel>(responseEntity);
        }

        public Guid CreateOrUpdate(ResponseDetailModel responseModel)
        {
            return _responseRepository.Exists(responseModel.Id)
                ? Update(responseModel)!.Value
                : Create(responseModel);
        }

        public Guid Create(ResponseDetailModel responseModel)
        {
            var responseEntity = _mapper.Map<ResponseEntity>(responseModel);
            return _responseRepository.Insert(responseEntity);
        }

        public Guid? Update(ResponseDetailModel responseModel)
        {
            var responseEntity = _mapper.Map<ResponseEntity>(responseModel);
            return _responseRepository.Update(responseEntity);
        }

        public void Delete(Guid id)
        {
            _responseRepository.Remove(id);
        }
        public List<ResponseDetailModel> GetByQuestionId(Guid questionId)
        {
            var responseEntities = _responseRepository
                .GetAll()
                .Where(r => r.QuestionId == questionId)
                .ToList();

            return _mapper.Map<List<ResponseDetailModel>>(responseEntities);
        }
    }
}