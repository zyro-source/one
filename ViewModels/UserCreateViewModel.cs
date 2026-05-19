using System.ComponentModel.DataAnnotations;

namespace buildwave.ViewModels;

public class UserCreateViewModel
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public Guid RoleId { get; set; }
}