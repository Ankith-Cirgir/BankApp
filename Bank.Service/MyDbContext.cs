using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankApp.Model;
using Microsoft.Extensions.Configuration;

namespace BankApp.Service
{
    public class MyDbContext : DbContext
    {
        public DbSet<Bank> Banks { get; set; }

        public DbSet<Currency> Currency { get; set; }

        public DbSet<CustomerAccount> CustomerAccounts { get; set; }

        public DbSet<Transaction> Transaction { get; set; }

        public DbSet<StaffAccounts> StaffAccounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appSettings.json");
            var configuration = builder.Build();

            optionsBuilder.UseMySQL(configuration.GetConnectionString("DefaultConnectionString"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Bank>().ToTable("Banks");

            modelBuilder.Entity<CustomerAccount>().ToTable("customeraccounts");

            modelBuilder.Entity<StaffAccounts>().ToTable("staffaccounts");

            modelBuilder.Entity<Transaction>().ToTable("transactions");

            modelBuilder.Entity<Currency>().ToTable("currency");

            modelBuilder.Entity<Bank>(entity =>
            {
                entity.HasKey(e => e.BankId);

                entity.Property(e => e.BankName).IsRequired();

                entity.Property(e => e.sRTGSCharge).IsRequired();

                entity.Property(e => e.sIMPSCharge).IsRequired();

                entity.Property(e => e.oRTGSCharge).IsRequired();
                
                entity.Property(e => e.oIMPSCharge).IsRequired();
                
                entity.Property(e => e.Profits);
            });

            modelBuilder.Entity<CustomerAccount>(entity =>
            {
                entity.HasKey(d => d.AccountId);

                entity.Property(d => d.BankId).IsRequired();

                entity.Property(d => d.Balance).IsRequired();

                entity.Property(d => d.Name).IsRequired();

                entity.Property(d => d.Password).IsRequired();

                entity.Property(d => d.IsActive);

            });

            modelBuilder.Entity<StaffAccounts>(entity =>
            {
                entity.HasKey(d => d.AccountId);

                entity.Property(d => d.Name).IsRequired();

                entity.Property(d => d.Password).IsRequired();

            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(d => d.TransactionId);

                entity.Property(d => d.SenderId);

                entity.Property(d => d.ReceiverId);

                entity.Property(d => d.Amount);

                entity.Property(d => d.Type).IsRequired();

                entity.Property(d => d.Time).IsRequired();

            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasKey(d => d.currency);

                entity.Property(d => d.value).IsRequired();

                entity.Property(d => d.BankId).IsRequired();

            });
        }
    }
}
