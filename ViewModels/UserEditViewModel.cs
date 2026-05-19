using System.ComponentModel.DataAnnotations;

namespace buildwave.ViewModels;

public class UserEditViewModel
{
    public Guid Id { get; set; }

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    [Required]
    public Guid RoleId { get; set; }
}