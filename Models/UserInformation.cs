namespace bloggy_ui.Models;

/// <summary>
/// Represents User Information for the currently signed on user.
/// </summary>
public class UserInformation {
    public string Id { get; set;}
    public string UserName { get; set;}
    public string Email { get; set;}
    public string FirstName { get; set;}
    public string LastName { get; set;}
    public string DateOfBirth { get; set;}
    public string UserRoleId { get; set;}
}