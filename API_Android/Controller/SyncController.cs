using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_Android.Data;
using API_Android.Models;

namespace API_Android.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SyncController(AppDbContext context)
        {
            _context = context;
        }

        // --- MODEL GÓI DỮ LIỆU ĐỒNG BỘ TỪ ANDROID LÊN CLOUD ---
        public class SyncPayload
        {
            public string UserId { get; set; } = string.Empty;
            public UserProfile? UserProfile { get; set; }
            public List<FoodLog> FoodLogs { get; set; } = new();
            public List<WaterLog> WaterLogs { get; set; } = new();
            public List<WeightEntry> WeightEntries { get; set; } = new();
            public List<WorkoutLog> WorkoutLogs { get; set; } = new();
        }

        // 1. SAO LƯU / ĐẨY DỮ LIỆU TỪ ANDROID LÊN CLOUD (UPLOAD)
        [HttpPost("upload")]
        public async Task<IActionResult> UploadBackup([FromBody] SyncPayload payload)
        {
            if (payload == null || string.IsNullOrEmpty(payload.UserId))
                return BadRequest(new { success = false, message = "Thiếu thông tin UserId." });

            // 1. Lưu User Profile
            if (payload.UserProfile != null)
            {
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserId == payload.UserId);
                if (profile == null)
                {
                    payload.UserProfile.UserId = payload.UserId;
                    _context.UserProfiles.Add(payload.UserProfile);
                }
                else
                {
                    profile.Name = payload.UserProfile.Name;
                    profile.Age = payload.UserProfile.Age;
                    profile.Gender = payload.UserProfile.Gender;
                    profile.Height = payload.UserProfile.Height;
                    profile.TargetWeight = payload.UserProfile.TargetWeight;
                    profile.ActivityLevel = payload.UserProfile.ActivityLevel;
                }
            }

            // 2. Đồng bộ Food Logs
            foreach (var item in payload.FoodLogs)
            {
                item.UserId = payload.UserId;
                item.Id = 0; // Đặt về 0 để SQL Server tự sinh ID tự tăng tránh lỗi Identity Insert
                _context.FoodLogs.Add(item);
            }

            // 3. Đồng bộ Water Logs
            foreach (var item in payload.WaterLogs)
            {
                item.UserId = payload.UserId;
                item.Id = 0;
                _context.WaterLogs.Add(item);
            }

            // 4. Đồng bộ Weight Entries
            foreach (var item in payload.WeightEntries)
            {
                item.UserId = payload.UserId;
                item.Id = 0;

                var existingWeight = await _context.WeightEntries
                    .FirstOrDefaultAsync(w => w.UserId == payload.UserId && w.RecordedDate.Date == item.RecordedDate.Date);
                if (existingWeight == null)
                {
                    _context.WeightEntries.Add(item);
                }
                else
                {
                    existingWeight.Weight = item.Weight;
                }
            }

            // 5. Đồng bộ Workout Logs
            foreach (var item in payload.WorkoutLogs)
            {
                item.UserId = payload.UserId;
                item.Id = 0;
                _context.WorkoutLogs.Add(item);
            }

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Sao lưu dữ liệu lên Cloud thành công!" });
        }

        // 2. KHÔI PHỤC / TẢI DỮ LIỆU TỪ CLOUD VỀ MÁY (DOWNLOAD - Khi chuyển thiết bị)
        [HttpGet("download/{userId}")]
        public async Task<IActionResult> DownloadData(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new { success = false, message = "Thiếu UserId." });

            // Sử dụng LINQ truy vấn toàn bộ dữ liệu thuộc về riêng userId đó
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserId == userId);
            var foodLogs = await _context.FoodLogs.Where(f => f.UserId == userId).ToListAsync();
            var waterLogs = await _context.WaterLogs.Where(w => w.UserId == userId).ToListAsync();
            var weightEntries = await _context.WeightEntries.Where(we => we.UserId == userId).ToListAsync();
            var workoutLogs = await _context.WorkoutLogs.Where(wo => wo.UserId == userId).ToListAsync();

            var cloudData = new
            {
                success = true,
                UserProfile = profile,
                FoodLogs = foodLogs,
                WaterLogs = waterLogs,
                WeightEntries = weightEntries,
                WorkoutLogs = workoutLogs
            };

            return Ok(cloudData);
        }
    }
}