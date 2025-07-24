using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Mubank.Models;
using Mubank.Models.ModelsHaveShip;

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
        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<CardModel> Cards { get; set; }
        public DbSet<HostConnectLogModel> HostConnectLog { get; set; }
        public DbSet<IPsBlockedModel> IPsBlocked { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
                .HasOne(a => a.Account)
                .WithOne(u => u.UserAccount)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AccountModel>()
                .HasOne(a => a.Card)
                .WithOne(c => c.Account)
                .HasForeignKey<CardModel>(c => c.AccountId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<AccountModel>()
                .HasMany(t => t.TransationsRecived)
                .WithOne(t => t.AccountDeQuemFez)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AccountModel>()
                .HasMany(t => t.TransationsMade)
                .WithOne(t => t.AccountDeQuemRecebeu)
                .OnDelete(DeleteBehavior.SetNull);

            base.OnModelCreating(modelBuilder);
        }
  
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
