using bloggy_ui.Models;
using Microsoft.AspNetCore.Components;

namespace bloggy_ui.Pages;

public partial class Register: ComponentBase {
    private UserRegisterRequest userRegisterRequest = new UserRegisterRequest();
    private bool registrationSuccess = false;

    private async Task HandleValidSubmit()
    {
        // You can send the userRegisterRequest object via HTTP request here
        // For now, let's just simulate a successful registration
        registrationSuccess = true;

        Console.WriteLine(userRegisterRequest);
    }
}