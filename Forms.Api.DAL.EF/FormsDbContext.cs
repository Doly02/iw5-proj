using Forms.Api.DAL.Common.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Forms.Api.DAL.Memory;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Forms.Api.DAL.EF;

public class FormsDbContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<QuestionEntity> Questions { get; set; } = null!;
    public DbSet<FormEntity> Forms { get; set; } = null!;
    public DbSet<ResponseEntity> Responses { get; set; } = null!;
    
    public FormsDbContext(DbContextOptions<FormsDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<UserEntity>()
            .HasMany(userEntity => userEntity.Forms)
            .WithOne()
            .HasForeignKey(formEntity => formEntity.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FormEntity>()
            .HasMany(formEntity => formEntity.Questions)
            .WithOne()  
            .HasForeignKey(questionEntity => questionEntity.FormId) 
            .OnDelete(DeleteBehavior.Cascade);
        

        // Entity Framework doesn’t know how to compare the List<string> properties (Answer and UserResponse) when checking for changes
        // Create a reusable ValueComparer for List<string>
        var listComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2), // Compare elements in the lists
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), // Compute hash code
            c => c.ToList() // Deep copy the list
        );
        
        // Serializes List<string> to JSON format for storage in the database
        modelBuilder.Entity<QuestionEntity>()
            .Property(q => q.Answer)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null), // Serialize List<string> to JSON
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ??
                     new List<string>() // Deserialize JSON to List<string>
            )
            .Metadata.SetValueComparer(listComparer);

        
        modelBuilder.Entity<ResponseEntity>()
            .Property(r => r.UserResponse)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ??
                     new List<string>()
            )
            .Metadata.SetValueComparer(listComparer);
        
    }

    public void SeedData()
    {
        Users.RemoveRange(Users);
        Forms.RemoveRange(Forms);
        Questions.RemoveRange(Questions);
        Responses.RemoveRange(Responses);
        SaveChanges();
        
        var storage = new Storage();

        if (!Users.Any())
        {
            Users.AddRange(storage.Users);
            SaveChanges();
        }
        if (!Forms.Any())
        {
            Forms.AddRange(storage.Forms);
            SaveChanges();
        }
        if (!Questions.Any())
        {
            Questions.AddRange(storage.Questions);
            SaveChanges();
        }
        if (!Responses.Any())
        {
            Responses.AddRange(storage.Responses);
            SaveChanges();
        }
    }
}