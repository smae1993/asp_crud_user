using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserApi.Data;
using UserApi.DTOs;
using UserApi.Models;

namespace UserApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(AppDbContext db) : ControllerBase
{
    // GET: api/users?page=1&pageSize=20
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

        var query = db.Users.AsNoTracking().OrderBy(u => u.Id);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            })
            .ToListAsync();

        return Ok(items);
    }

    // GET: api/users/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var u = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (u is null) return NotFound();

        return Ok(new UserDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        });
    }

    // POST: api/users
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserDto dto)
    {
        // ایمیل تکراری؟
        var exists = await db.Users.AnyAsync(x => x.Email == dto.Email);
        if (exists) return Conflict(new { message = "Email already exists." });

        var entity = new User
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim().ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(entity);
        await db.SaveChangesAsync();

        var result = new UserDto
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            CreatedAt = entity.CreatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
    }

    // PUT: api/users/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> Update(int id, UpdateUserDto dto)
    {
        var u = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (u is null) return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.FirstName)) u.FirstName = dto.FirstName.Trim();
        if (!string.IsNullOrWhiteSpace(dto.LastName))  u.LastName = dto.LastName.Trim();

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var email = dto.Email.Trim().ToLower();
            var exists = await db.Users.AnyAsync(x => x.Email == email && x.Id != id);
            if (exists) return Conflict(new { message = "Email already exists." });
            u.Email = email;
        }

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            u.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        u.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Ok(new UserDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        });
    }

    // DELETE: api/users/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var u = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (u is null) return NotFound();

        db.Users.Remove(u);
        await db.SaveChangesAsync();

        return NoContent();
    }
}
