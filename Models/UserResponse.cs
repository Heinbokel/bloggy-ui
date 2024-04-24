using System.Text.Json.Serialization;
namespace Bloggy.Models;

/// <summary>
/// Represents a User of Bloggy, returned from the Bloggy API.
/// </summary>
public class UserResponse {

    public int Id { get; set;}

    public string UserName { get; set;}
    public string Email { get; set;}
    public string FirstName { get; set;}
    public string LastName { get; set;}
    public DateOnly DateOfBirth { get; set;}
    public int UserRoleId { get; set;}

}