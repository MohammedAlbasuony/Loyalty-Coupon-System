using LoyaltyCouponsSystem.DAL.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyCouponsSystem.DAL.DB
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<GlobalCounter> GlobalCounters { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<StoreKeeper> StoreKeepers { get; set; }

        public DbSet<Technician> Technicians { get; set; }
        public DbSet<Distributor> Distributors { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Representative> Representatives { get; set; }

        public DbSet<Governorate> Governorates { get; set; }

        public DbSet<Area> Areas { get; set; }

        public DbSet<QRScanLog> QRScanLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Always call base first for Identity configuration

            // Ensure primary key for IdentityUserLogin<string>
            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.HasKey(l => new { l.LoginProvider, l.ProviderKey });
            });

         

            modelBuilder.Entity<DistributorCustomer>()
                .HasKey(dc => new { dc.DistributorID, dc.CustomerID });

            modelBuilder.Entity<DistributorCustomer>()
                .HasOne(dc => dc.Distributor)
                .WithMany(d => d.DistributorCustomers)
                .HasForeignKey(dc => dc.DistributorID)
                .IsRequired(false); // Make the relationship optional


            modelBuilder.Entity<DistributorCustomer>()
                .HasOne(dc => dc.Customer)
                .WithMany(c => c.DistributorCustomers)
                .HasForeignKey(dc => dc.CustomerID);


            //Governate and Area
            modelBuilder.Entity<Governorate>()
                .HasMany(g => g.Areas)
                .WithOne(a => a.Governorate)
                .HasForeignKey(a => a.GovernateId)
                .OnDelete(DeleteBehavior.Cascade); // عند حذف المحافظة يتم حذف المناطق

            // Ensure NationalID is unique
            modelBuilder.Entity<ApplicationUser>()
                    .HasIndex(u => u.NationalID)
                    .IsUnique();

            // Ensure PhoneNumber is unique
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<Admin>()
                .HasOne(d => d.ApplicationUser)
                .WithOne(au => au.Admin)
                .HasForeignKey<Admin>(d => d.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Representative>()
                .HasOne(d => d.ApplicationUser)
                .WithOne(au => au.Representative)
                .HasForeignKey<Representative>(d => d.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerID);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Governate)
                    .HasMaxLength(50);

                entity.Property(e => e.City)
                    .HasMaxLength(50);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(11);
            });


            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.TransactionID);
                entity.Property(e => e.TransactionType).HasMaxLength(50);
                entity.Property(e => e.PurchaseAmount).HasColumnType("decimal(18,2)");
                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Transactions)
                    .HasForeignKey(e => e.CustomerID);
            });


            modelBuilder.Entity<GlobalCounter>(entity =>
            {
                entity.HasKey(GC => GC.Year);
            });


            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.HasKey(e => e.CouponeId);

                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<StoreKeeper>(entity =>
            {
                entity.HasKey(e => e.StoreKeeperID);
                entity.Property(e => e.NameAttribute).IsRequired().HasMaxLength(100);
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

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.AuditLogID);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(200);
            });
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is ApplicationUser && e.State == EntityState.Deleted);

            foreach (var entry in entries)
            {
                entry.State = EntityState.Modified;
                ((ApplicationUser)entry.Entity).IsDeleted = true;
            }

            return base.SaveChanges();
        }
    }
}
