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
        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<TransationsModel> Transations { get; set; }
        public DbSet<ErrorModel> Errors { get; set; }
        public DbSet<HostConnectLogModel> HostConnectLog { get; set; }
        public DbSet<IPsBlockedModel> IPsBlocked { get; set; }
        public DbSet<UserDataFULLModel> UserDataFULL { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1:1 entre User e Account
            modelBuilder.Entity<UserModel>()
                .HasOne(u => u.Account)
                .WithOne(a => a.User)
                .HasForeignKey<AccountModel>(a => a.UserId);

            // 1:N - Transações enviadas
            modelBuilder.Entity<TransationsModel>()
                .HasOne(t => t.Remetente)
                .WithMany(a => a.TransaçõesEnviadas)
                .HasForeignKey(t => t.RemetenteId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1:N - Transações recebidas
            modelBuilder.Entity<TransationsModel>()
                .HasOne(t => t.Destinatario)
                .WithMany(a => a.TransaçõesRecebidas)
                .HasForeignKey(t => t.DestinatarioId)
                .OnDelete(DeleteBehavior.Restrict);                                            

            modelBuilder.Entity<UserDataFULLModel>()
                .HasOne(u => u.User)
                .WithMany()
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);
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
