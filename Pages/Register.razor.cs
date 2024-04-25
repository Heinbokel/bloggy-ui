using Bloggy.Models;
using bloggy_ui.Models;
using bloggy_ui.Services;
using Microsoft.AspNetCore.Components;

namespace bloggy_ui.Pages;

/// <summary>
/// Page that handles user registration.
/// </summary>
public partial class Register: ComponentBase {
    [Inject]
    private AuthService _authService {get; set;}

    [Inject]
    private NavigationManager _navigationManager {get; set;}

    private UserRegisterRequest userRegisterRequest = new UserRegisterRequest();
    private bool errorOccured = false;

    private async Task HandleValidSubmit()
    {
        this.errorOccured = false;
        UserResponse? registeredUser = await this._authService.RegisterAsync(userRegisterRequest);
        if (registeredUser != null) {
            this._navigationManager.NavigateTo("/login");
        } else {
            this.errorOccured = true;
        }
    }
}