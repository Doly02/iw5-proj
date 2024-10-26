using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;

namespace Forms.Api.DAL.Memory.Repositories
{
    public class ResponseRepository : IResponseRepository
    {
        private readonly IList<ResponseEntity> _responses;
        private readonly IMapper _mapper;

        public ResponseRepository(
            Storage storage,
            IMapper mapper)
        {
            _responses = storage.Responses;
            _mapper = mapper;
        }

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
            var responseExists = _responses.SingleOrDefault(responseInStore =>
                responseInStore.Id == entity.Id);
            if (null != responseExists) {
                _mapper.Map(entity, responseExists);
            }
            return responseExists?.Id;
        }

        public void Remove(Guid id)
        {
            var responseToRemove = _responses.Single(response => response.Id.Equals(id));
            _responses.Remove(responseToRemove);
        }

        public bool Exists(Guid id)
        {
            return _responses.Any(response => response.Id == id);
        }
    }
}