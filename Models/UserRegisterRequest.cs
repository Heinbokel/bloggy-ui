using System.ComponentModel.DataAnnotations;

namespace bloggy_ui.Models;

/// <summary>
/// Represents the request to register a new user.
/// </summary>
public class UserRegisterRequest {

    [Required]
    [MinLength(2)]
    [MaxLength(64)]
    public string UserName { get; set;}

    [Required]
    [EmailAddress]
    public string Email { get; set;}

    [Required]
    [MinLength(8)]
    [MaxLength(256)]
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
    public int UserRoleId { get; set;}

}