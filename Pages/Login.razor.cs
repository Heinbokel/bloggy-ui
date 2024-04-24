using System.Net;
using System.Net.Http.Json;
using bloggy_ui.Models;
using Microsoft.AspNetCore.Components;

namespace bloggy_ui.Pages;

public partial class Login: ComponentBase {
    [Inject]
    private HttpClient httpClient {get; set;}

    [Inject]
    private NavigationManager navigationManager {get; set;}

    private static string BASE_URL = "http://localhost:5000";
    private LoginRequest loginRequest = new LoginRequest();

    private bool errorOccured = false;
    private bool invalidCredentials = false;

    private async Task SubmitForm()
    {
        this.errorOccured = false;
        this.invalidCredentials = false;
        try {
            HttpResponseMessage httpResponseMessage = await this.httpClient.PostAsJsonAsync($"{BASE_URL}/login", this.loginRequest);

            if (httpResponseMessage.IsSuccessStatusCode) {   
                // Handle the JWT contained in the login response             
                // Navigate to home page.
                this.navigationManager.NavigateTo("/");
            } else if (httpResponseMessage.StatusCode == HttpStatusCode.Forbidden) {
                this.invalidCredentials = true;
            } else {
                this.errorOccured = true;
            }
        } catch (HttpRequestException exception) {
            this.errorOccured = true;
            Console.WriteLine(exception.ToString());
        }
    }
}