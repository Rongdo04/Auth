using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace PhanQuyenAPI.Models {
    public class AppDbContext : DbContext {
        public virtual DbSet<Accounts> Account { get; set; }
        public virtual DbSet<Decentralizations> Decentralization { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            // Cấu hình kết nối đến cơ sở dữ liệu, ví dụ: SQL Server
            optionsBuilder.UseSqlServer("Server=DESKTOP-KBA58DG\\SQLEXPRESS;Database=PhanQuyenAPI;Trusted_Connection = true;TrustServerCertificate = true");

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Accounts>()
                .HasIndex(e => e.userName)
                .IsUnique();
        }

    }
}
