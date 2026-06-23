using EventBooking.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EventBooking.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<PermissionDetail> PermissionDetails { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<VendorProfile> VendorProfiles { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ServicePackage> ServicePackages { get; set; }
        public DbSet<AvailableSlot> AvailableSlots { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<RevenueReport> RevenueReports { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<PortfolioImage> PortfolioImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- PRIMARY KEYS ---
            modelBuilder.Entity<PermissionDetail>().HasKey(p => p.PermissionId);
            modelBuilder.Entity<VendorProfile>().HasKey(v => v.VendorId);
            modelBuilder.Entity<ServicePackage>().HasKey(sp => sp.PackageId);
            modelBuilder.Entity<AvailableSlot>().HasKey(a => a.SlotId);
            modelBuilder.Entity<SystemLog>().HasKey(sl => sl.LogId);
            modelBuilder.Entity<RevenueReport>().HasKey(rr => rr.ReportId);
            modelBuilder.Entity<PortfolioImage>().HasKey(pi => pi.ImageId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // --- CẤU HÌNH QUAN HỆ (RELATIONSHIPS) ---

            //// 1. Portfolio -> VendorProfile (Fix lỗi khóa ngoại 'VendorProfileVendorId')
            modelBuilder.Entity<Portfolio>()
                .HasOne(p => p.VendorProfile)
                .WithMany()
                .HasForeignKey(p => p.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. VendorProfile -> User (One-to-One)
            modelBuilder.Entity<VendorProfile>()
                .HasOne(v => v.User)
                .WithOne(u => u.VendorProfile)
                .HasForeignKey<VendorProfile>(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Payment -> Booking (One-to-One)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. Review -> Booking (One-to-One)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Booking)
                .WithOne(b => b.Review)
                .HasForeignKey<Review>(r => r.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            // 5. Booking -> Customer (One-to-Many)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // 6. Booking -> ServicePackage (One-to-Many)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.ServicePackage)
                .WithMany(sp => sp.Bookings)
                .HasForeignKey(b => b.PackageId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- KIỂU DỮ LIỆU SỐ THẬP PHÂN ---
            modelBuilder.Entity<ServicePackage>().Property(sp => sp.Price).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Booking>().Property(b => b.TotalAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Booking>().Property(b => b.DepositAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<RevenueReport>().Property(r => r.TotalRevenue).HasColumnType("decimal(18,2)");

            // --- DEFAULT VALUES ---
            modelBuilder.Entity<User>().Property(u => u.IsActive).HasDefaultValue(true);
            modelBuilder.Entity<ServicePackage>().Property(sp => sp.IsActive).HasDefaultValue(true);
            modelBuilder.Entity<Booking>().Property(b => b.Status).HasDefaultValue(0);
        }
    }
}