using System.ComponentModel.DataAnnotations;

namespace TaskApi.Dtos;

public class RegisterDto
{
    [Required, MinLength(3), MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required, MinLength(6), MaxLength(128)]
    public string Password { get; set; } = string.Empty;
}

public class LoginDto
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
}
