using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Forms.Api.DAL.EF.Factories
{
    public class FormsDbContextFactory : IDesignTimeDbContextFactory<FormsDbContext>
    {
        public FormsDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<FormsDbContextFactory>(optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<FormsDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            return new FormsDbContext(optionsBuilder.Options);
        }
    }
}