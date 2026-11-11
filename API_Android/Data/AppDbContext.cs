using Microsoft.EntityFrameworkCore;
using API_Android.Models;

namespace API_Android.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Khai báo bảng Users để hết lỗi CS1061
        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<FoodLog> FoodLogs { get; set; }
        public DbSet<WaterLog> WaterLogs { get; set; }
        public DbSet<WeightEntry> WeightEntries { get; set; }
        public DbSet<WorkoutLog> WorkoutLogs { get; set; }
    }
}