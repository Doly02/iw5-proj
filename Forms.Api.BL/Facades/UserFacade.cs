using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;
using Forms.Common.Models.Search;
using Forms.Common.Models.User;

namespace Forms.Api.BL.Facades;

public class UserFacade : FacadeBase<IUserRepository, UserEntity>, IUserFacade
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;

    public UserFacade(IUserRepository userRepository, IMapper mapper)
        : base(userRepository) 
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
    }

    public List<UserListModel> GetAll()
    {
        var userEntities = userRepository.GetAll();
        return mapper.Map<List<UserListModel>>(userEntities);
    }
    
    public UserDetailModel? GetById(Guid id)
    {
        var userEntity = userRepository.GetById(id);
        return mapper.Map<UserDetailModel>(userEntity);
    }

    public Guid CreateOrUpdate(UserDetailModel userModel, string? ownerId)
    {
        return userRepository.Exists(userModel.Id)
            ? Update(userModel, ownerId)!.Value
            : Create(userModel);
    }

    public Guid Create(UserDetailModel userModel)
    {
        var userEntity = mapper.Map<UserEntity>(userModel);
        userEntity.OwnerId = userModel.Id.ToString();
        
        return userRepository.Insert(userEntity);
    }

    public Guid? Update(UserDetailModel userModel, string? ownerId)
    {
        ThrowIfWrongOwner(userModel.Id, ownerId);
        
        var userEntity = mapper.Map<UserEntity>(userModel);
        
        var result = userRepository.Update(userEntity);
        return result;
    }

    public void Delete(Guid id, string? ownerId)
    {
        ThrowIfWrongOwner(id, ownerId);
        
        userRepository.Remove(id);
    }
    
    public async Task<List<SearchResultModel>> SearchAsync(string query)
    {
        var users = await userRepository.SearchAsync(query);
        return mapper.Map<List<SearchResultModel>>(users);
    }
}
