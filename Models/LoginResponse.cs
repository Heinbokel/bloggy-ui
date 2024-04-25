namespace bloggy_ui.Models;

/// <summary>
/// Represents the response from a successful login, containing the JWT as a string.
/// </summary>
public class LoginResponse {

    public string Token {get; set;}

}