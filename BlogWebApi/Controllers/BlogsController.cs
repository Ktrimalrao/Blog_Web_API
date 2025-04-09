using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogWebApi.Data;
using BlogWebApi.DTOs;
using BlogWebApi.Models;
using BlogWebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using static BlogWebApi.DTOs.BlogDTOs;

namespace BlogWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public BlogsController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // GET: api/Blogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogResponseDTO>>> GetBlogs()
        {
            var blogs = await _context.Blogs
                .Include(b => b.User)
                .OrderByDescending(b => b.CreatedDate)
                .Select(b => new BlogResponseDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    Content = b.Content,
                    CreatedDate = b.CreatedDate,
                    AuthorName = b.User.Name,
                    UserId = b.UserId
                })
                .ToListAsync();

            return Ok(blogs);
        }

        // GET: api/Blogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogResponseDTO>> GetBlog(int id)
        {
            var blog = await _context.Blogs
                .Include(b => b.User)
                .Where(b => b.Id == id)
                .Select(b => new BlogResponseDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    Content = b.Content,
                    CreatedDate = b.CreatedDate,
                    AuthorName = b.User.Name,
                    UserId = b.UserId
                })
                .FirstOrDefaultAsync();

            if (blog == null)
            {
                return NotFound();
            }

            return blog;
        }

        // GET: api/Blogs/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<BlogResponseDTO>>> GetUserBlogs(int userId)
        {
            var blogs = await _context.Blogs
                .Include(b => b.User)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedDate)
                .Select(b => new BlogResponseDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    Content = b.Content,
                    CreatedDate = b.CreatedDate,
                    AuthorName = b.User.Name,
                    UserId = b.UserId
                })
                .ToListAsync();

            return Ok(blogs);
        }
        //[Authorize]
        //// POST: api/Blogs
        //[HttpPost]
        //public async Task<ActionResult<BlogResponseDTO>> CreateBlog(BlogCreateDTO dto)
        //{
        //    try
        //    {
        //        // Get current user ID from JWT
        //        var jwt = Request.Cookies["jwt"];
        //        if (jwt == null)
        //        {
        //            return Unauthorized(new { message = "Unauthorized" });
        //        }

        //        var token = _jwtService.VerifyToken(jwt);
        //        var userId = int.Parse(token.Claims.First(c => c.Type == "id").Value);

        //        var blog = new Blog
        //        {
        //            Title = dto.Title,
        //            Content = dto.Content,
        //            CreatedDate = DateTime.UtcNow,
        //            UserId = userId
        //        };

        //        _context.Blogs.Add(blog);
        //        await _context.SaveChangesAsync();

        //        // Fetch the user to get the name
        //        var user = await _context.Users.FindAsync(userId);

        //        return CreatedAtAction(nameof(GetBlog), new { id = blog.Id }, new BlogResponseDTO
        //        {
        //            Id = blog.Id,
        //            Title = blog.Title,
        //            Content = blog.Content,
        //            CreatedDate = blog.CreatedDate,
        //            AuthorName = user.Name,
        //            UserId = blog.UserId
        //        });
        //    }
        //    catch (Exception)
        //    {
        //        return Unauthorized(new { message = "Unauthorized" });
        //    }
        //}
        [Authorize]
        // POST: api/Blogs
        [HttpPost]
        public async Task<ActionResult<BlogResponseDTO>> CreateBlog(BlogCreateDTO dto)
        {
            try
            {
                // Retrieve the token from the Authorization header
                var authHeader = Request.Headers["Authorization"].ToString();

                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { message = "Unauthorized" });
                }

                // Extract the token from the Authorization header
                var token = authHeader.Substring("Bearer ".Length).Trim();

                // Verify the token
                var claimsPrincipal = _jwtService.VerifyToken(token);

                // Extract user ID from the claims
                var userId = int.Parse(claimsPrincipal.Claims.First(c => c.Type == "id").Value);

                var blog = new Blog
                {
                    Title = dto.Title,
                    Content = dto.Content,
                    CreatedDate = DateTime.UtcNow,
                    UserId = userId
                };

                _context.Blogs.Add(blog);
                await _context.SaveChangesAsync();

                // Fetch the user to get the name
                var user = await _context.Users.FindAsync(userId);

                return CreatedAtAction(nameof(GetBlog), new { id = blog.Id }, new BlogResponseDTO
                {
                    Id = blog.Id,
                    Title = blog.Title,
                    Content = blog.Content,
                    CreatedDate = blog.CreatedDate,
                    AuthorName = user.Name,
                    UserId = blog.UserId
                });
            }
            catch (Exception ex)
            {
                // Log the exception and return unauthorized
                
                return Unauthorized(new { message = "Unauthorized", error = ex.Message });
            }
        }


        // PUT: api/Blogs/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlog(int id, BlogUpdateDTO dto)
        {
            try
            {
                // Get JWT token from Authorization header
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { message = "Unauthorized" });
                }

                // Extract the token from the Authorization header
                var token = authHeader.Substring("Bearer ".Length).Trim();

                // Verify the token and get the user ID
                var claimsPrincipal = _jwtService.VerifyToken(token);
                var userId = int.Parse(claimsPrincipal.Claims.First(c => c.Type == "id").Value);

                var blog = await _context.Blogs.FindAsync(id);
                if (blog == null)
                {
                    return NotFound();
                }

                // Check if the user is the author of the blog
                if (blog.UserId != userId)
                {
                    return Forbid();
                }

                // Update the blog content
                blog.Title = dto.Title;
                blog.Content = dto.Content;

                _context.Entry(blog).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return Unauthorized(new { message = "Unauthorized", error = ex.Message });
            }
        }


        // DELETE: api/Blogs/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            try
            {
                // Get JWT token from Authorization header
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { message = "Unauthorized" });
                }

                // Extract the token from the Authorization header
                var token = authHeader.Substring("Bearer ".Length).Trim();

                // Verify the token and get the user ID
                var claimsPrincipal = _jwtService.VerifyToken(token);
                var userId = int.Parse(claimsPrincipal.Claims.First(c => c.Type == "id").Value);

                var blog = await _context.Blogs.FindAsync(id);
                if (blog == null)
                {
                    return NotFound();
                }

                // Check if the user is the author of the blog
                if (blog.UserId != userId)
                {
                    return Forbid();
                }

                _context.Blogs.Remove(blog);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return Unauthorized(new { message = "Unauthorized", error = ex.Message });
            }
        }

    }
}