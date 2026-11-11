using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_Android.Data;
using API_Android.Models;

namespace API_Android.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        public class AuthRequestModel
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        // 1. ĐĂNG KÝ (Không dùng Hash)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRequestModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest(new { success = false, message = "Tài khoản và mật khẩu không được để trống." });

            bool userExists = await _context.Users.AnyAsync(u => u.Username == model.Username);
            if (userExists)
                return BadRequest(new { success = false, message = "Tên đăng nhập đã tồn tại." });

            var newUser = new User
            {
                UserId = "USR_" + Guid.NewGuid().ToString().Substring(0, 8),
                Username = model.Username,
                Password = model.Password // Lưu mật khẩu dạng chuỗi bình thường
            };

            _context.Users.Add(newUser);

            // Tạo sẵn một dòng UserProfile mặc định
            var newProfile = new UserProfile
            {
                UserId = newUser.UserId,
                Name = model.Username
            };
            _context.UserProfiles.Add(newProfile);

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Đăng ký thành công!", userId = newUser.UserId });
        }

        // 2. ĐĂNG NHẬP (So sánh trực tiếp chuỗi mật khẩu)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequestModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username && u.Password == model.Password);

            if (user == null)
            {
                return Unauthorized(new { success = false, message = "Sai tên đăng nhập hoặc mật khẩu." });
            }

            return Ok(new
            {
                success = true,
                message = "Đăng nhập thành công!",
                userId = user.UserId,
                username = user.Username
            });
        }
    }
}