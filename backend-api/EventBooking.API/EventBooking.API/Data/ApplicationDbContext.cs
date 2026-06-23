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

        // ========================================================
        // 1. KHAI BÁO CÁC DBSET (Tương đương với các bảng trong SQL)
        // ========================================================
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

        // ========================================================
        // 2. CẤU HÌNH FLUENT API (Ràng buộc, Khóa ngoại, Kiểu dữ liệu)
        // ========================================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            // --- CẤU HÌNH QUAN HỆ 1-1 (One-to-One) ---

            // 1 User (Vendor) chỉ có 1 VendorProfile
            modelBuilder.Entity<VendorProfile>()
                .HasOne(v => v.User)
                .WithOne(u => u.VendorProfile)
                .HasForeignKey<VendorProfile>(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh lỗi Multiple Cascade Paths của SQL Server

            // 1 Booking chỉ có 1 Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1 Booking chỉ có 1 Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Booking)
                .WithOne(b => b.Review)
                .HasForeignKey<Review>(r => r.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- CẤU HÌNH QUAN HỆ 1-N (Đặc biệt cho Booking) ---

            // User (Customer) đặt nhiều Bookings
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ServicePackage nằm trong nhiều Bookings
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.ServicePackage)
                .WithMany(sp => sp.Bookings)
                .HasForeignKey(b => b.PackageId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- CẤU HÌNH KIỂU DỮ LIỆU SỐ THẬP PHÂN (Decimal Precision) ---
            // Tránh warning "decimal column will be truncated" của EF Core

            modelBuilder.Entity<ServicePackage>()
                .Property(sp => sp.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Booking>()
                .Property(b => b.DepositAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<RevenueReport>()
                .Property(r => r.TotalRevenue)
                .HasColumnType("decimal(18,2)");

            // --- CẤU HÌNH DEFAULT VALUES MẶC ĐỊNH ---
            modelBuilder.Entity<User>()
                .Property(u => u.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<ServicePackage>()
                .Property(sp => sp.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Booking>()
                .Property(b => b.Status)
                .HasDefaultValue(0);
        }
    }
}