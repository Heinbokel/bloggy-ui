using System.Net.Http.Json;
using bloggy_ui.Models;
using Microsoft.AspNetCore.Components;

namespace bloggy_ui.Pages;

public partial class Register: ComponentBase {
    [Inject]
    private HttpClient httpClient {get; set;}

    [Inject]
    private NavigationManager navigationManager {get; set;}

    private static string BASE_URL = "http://localhost:5000";
    private UserRegisterRequest userRegisterRequest = new UserRegisterRequest();
    private bool errorOccured = false;

    private async Task HandleValidSubmit()
    {
        this.errorOccured = false;
        try {
            HttpResponseMessage httpResponseMessage = await this.httpClient.PostAsJsonAsync($"{BASE_URL}/users", this.userRegisterRequest);

            if (httpResponseMessage.IsSuccessStatusCode) {                
                // Navigate to login. We don't really need to use the response from registration at all.
                this.navigationManager.NavigateTo("/login");
            } else {
                this.errorOccured = true;
            }
        } catch (HttpRequestException exception) {
            this.errorOccured = true;
            Console.WriteLine(exception.ToString());
        }
    }
}