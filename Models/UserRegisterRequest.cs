using System.ComponentModel.DataAnnotations;

namespace bloggy_ui.Models;

/// <summary>
/// Represents the request to register a new user.
/// </summary>
public class UserRegisterRequest {

    [Required(ErrorMessage = "Username must be provided.")]
    [MinLength(2, ErrorMessage = "Username must be at least 2 characters.")]
    [MaxLength(64, ErrorMessage = "Username must be 64 characters or less.")]
    public string UserName { get; set;}

    [Required]
    [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
    public string Email { get; set;}

    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [MaxLength(256, ErrorMessage = "pASSWORD must be at 256 characters or less.")]
    public string Password { get; set;}

    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public string FirstName { get; set;}

    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public string LastName { get; set;}

    [Required]
    public DateOnly DateOfBirth { get; set;}

    [Required]
    [Range(1, 2, ErrorMessage = "User role ID must be 1 or 2.")]
    public int UserRoleId { get; set;}

}