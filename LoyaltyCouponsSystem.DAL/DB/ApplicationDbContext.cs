using LoyaltyCouponsSystem.DAL.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyCouponsSystem.DAL.DB
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {


        }
        public DbSet<Customer> Customers { get; set; } 
        public DbSet<Transaction> Transactions { get; set; } 
        public DbSet<Coupon> Coupons { get; set; } 
        public DbSet<StoreKeeper> StoreKeepers { get; set; } 
        public DbSet<CouponTemplate> CouponTemplates { get; set; }
        public DbSet<Technician> Technicians { get; set; }
        public DbSet<Employee> Employees { get; set; } 
        public DbSet<Admin> Admins { get; set; } 
        public DbSet<AuditLog> AuditLogs { get; set; } 
        public DbSet<Representative> Representatives { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerID);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ContactDetails).HasMaxLength(200);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.TransactionID);
                entity.Property(e => e.TransactionType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PurchaseAmount).HasColumnType("decimal(18,2)");
                entity.HasOne(e => e.Customer)
                      .WithMany(c => c.Transactions)
                      .HasForeignKey(e => e.CustomerID);
            });

            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.HasKey(e => e.CouponID);
                entity.Property(e => e.UniqueIdentifier).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<StoreKeeper>(entity =>
            {
                entity.HasKey(e => e.StoreKeeperID);
                entity.Property(e => e.NameAttribute).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<CouponTemplate>(entity =>
            {
                entity.HasKey(e => e.TemplateID);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Technician>(entity =>
            {
                entity.HasKey(e => e.TechnicianID);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeID);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.AdminID);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.AuditLogID);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<Representative>(entity =>
            {
                entity.HasKey(e => e.RepresentativeID);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });
        }

    }
}
