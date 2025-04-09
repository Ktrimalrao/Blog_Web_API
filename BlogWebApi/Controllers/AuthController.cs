using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BlogWebApi.DTOs;
using BlogWebApi.Models;
using BlogWebApi.Data;
using BlogWebApi.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static BlogWebApi.DTOs.AuthDTOs;

namespace BlogWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest(new { message = "All fields are required" });
            }

            // Check if email is already registered
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest(new { message = "Email already exists" });
            }

            // Create new user
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Return user info (without password)
            return Ok(new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest(new { message = "All fields are required" });
            }

            // Check if user exists
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid credentials" });
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                return BadRequest(new { message = "Invalid credentials" });
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);

            // Set cookie with token
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
                Secure = true
            });

            return Ok(new
            {
                message = "Login successful",
                user = new UserDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                },
                token = token
            });
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                // Get JWT from request
                var jwt = Request.Cookies["jwt"];
                if (jwt == null)
                {
                    return Unauthorized(new { message = "Unauthorized" });
                }

                var token = _jwtService.VerifyToken(jwt);
                var userId = int.Parse(token.Claims.First(c => c.Type == "id").Value);

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return Unauthorized(new { message = "Unauthorized" });
                }

                return Ok(new UserDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                });
            }
            catch (Exception)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt", new CookieOptions
            {
                HttpOnly = true,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
                Secure = true
            });

            return Ok(new { message = "Logged out successfully" });
        }
    }
}