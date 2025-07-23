using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Mubank.Models;

namespace Mubank.Services
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<TransationsModel> Transations { get; set; }
        public DbSet<ErrorModel> Errors { get; set; }
        public DbSet<HostConnectLogModel> HostConnectLog { get; set; }
        public DbSet<IPsBlockedModel> IPsBlocked { get; set; }

  
    }

    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer("Data Source=HENRIQZIN\\SQLEXPRESS;Initial Catalog=Mubank;Integrated Security=True;Pooling=False;Encrypt=True;Trust Server Certificate=True");

            return new DataContext(optionsBuilder.Options);
        }
    }
}
