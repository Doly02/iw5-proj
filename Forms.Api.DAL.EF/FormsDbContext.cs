using Forms.Api.DAL.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace Forms.Api.DAL.EF;

public class FormsDbContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<QuestionEntity> Questions => Set<QuestionEntity>(); 
    
    public FormsDbContext(DbContextOptions<FormsDbContext> options)
        : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntity>()
            .HasMany(userEntity => userEntity.Responses)
            .WithOne(responseEntity => responseEntity.User)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<UserEntity>()
            .HasMany(userEntity => userEntity.Forms)
            .WithOne()
            .HasForeignKey(formEntity => formEntity.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<FormEntity>()
            .HasMany(formEntity => formEntity.Questions)
            .WithOne(questionEntity => questionEntity.Form)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<QuestionEntity>()
            .HasMany(questionEntity => questionEntity.Responses)
            .WithOne(responseEntity => responseEntity.Question)
            .OnDelete(DeleteBehavior.Cascade);
        
        
    }
    
}