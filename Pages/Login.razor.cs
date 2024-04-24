using System.Net;
using System.Net.Http.Json;
using bloggy_ui.Models;
using bloggy_ui.Services;
using Microsoft.AspNetCore.Components;

namespace bloggy_ui.Pages;

public partial class Login: ComponentBase {
    [Inject]
    private NavigationManager _navigationManager {get; set;}

    [Inject]
    private AuthService _authService {get; set;}

    private LoginRequest loginRequest = new LoginRequest();

    private bool errorOccured = false;
    private bool invalidCredentials = false;

    private async Task SubmitForm()
    {
        this.errorOccured = false;
        this.invalidCredentials = false;
        HttpStatusCode responseCode = await this._authService.LoginAsync(loginRequest);

        if (responseCode == HttpStatusCode.OK) {
            this._navigationManager.NavigateTo("/user-profile");
        } else if (responseCode == HttpStatusCode.Forbidden) {
            this.invalidCredentials = true;
        } else {
            this.errorOccured = true;
        }
        
    }
}