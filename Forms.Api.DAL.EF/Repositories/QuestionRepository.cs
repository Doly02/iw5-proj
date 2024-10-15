using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Forms.Api.DAL.EF.Repositories;

public class QuestionRepository : RepositoryBase<QuestionEntity>, IQuestionRepository
{
    private readonly IMapper _mapper;
    
    public QuestionRepository(FormsDbContext context, IMapper mapper) : base(context)
    {
        this._mapper = mapper;
    }

    public override QuestionEntity? GetById(Guid id)
    {
        return context.Questions
            .Include(u => u.Responses) 
            .FirstOrDefault(u => u.Id == id); 
    }

    public override Guid? Update(QuestionEntity question)
    {
        if (Exists(question.Id))
        {
            var existQuestion = context.Questions
                .Include(u => u.Responses)
                .Single(u => u.Id == question.Id);
            
            _mapper.Map(question, existQuestion);
            
            context.Questions.Update(existQuestion);
            context.SaveChanges();
            return existQuestion.Id;
        }

        return null;
    }
}