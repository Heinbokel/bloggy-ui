using bloggy_ui.Models;
using bloggy_ui.Services;
using Microsoft.AspNetCore.Components;

namespace bloggy_ui.Pages;

/// <summary>
/// Component representing the User Profile page.
/// </summary>
public partial class UserProfile: ComponentBase {
    
    [Inject]
    private AuthService _authService {get; set;}

    [Inject]
    private NavigationManager _navigationManager {get; set;}

    private UserInformation UserInformation;

    private bool IsLoading {get; set;} = true;

    /// <summary>
    /// Logic to run in the OnInitialized lifecycle hook.
    /// </summary>
    /// <returns></returns>
    protected override async void OnInitialized() {
        this.IsLoading = true;
        this._authService.GetUserInformationAsync().Subscribe((userInfo) => {
            if (userInfo == null) {
                this._navigationManager.NavigateTo("/login");
            } else {
                this.UserInformation = userInfo;
                this.IsLoading = false;
                StateHasChanged();
            }
            base.OnInitialized();
        });
    }

}