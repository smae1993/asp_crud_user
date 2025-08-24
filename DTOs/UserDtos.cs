using System;
using System.ComponentModel.DataAnnotations;

namespace UserApi.DTOs;

public class CreateUserDto
{
    [Required, StringLength(50)]
    public string FirstName { get; set; } = null!;

    [Required, StringLength(50)]
    public string LastName { get; set; } = null!;

    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; } = null!;

    [Required, MinLength(6), StringLength(100)]
    public string Password { get; set; } = null!;
}

public class UpdateUserDto
{
    [StringLength(50)]
    public string? FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }

    [EmailAddress, StringLength(100)]
    public string? Email { get; set; }

    // اختیاری؛ اگر فرستاده شد، رمز عوض می‌شود
    [MinLength(6), StringLength(100)]
    public string? Password { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
