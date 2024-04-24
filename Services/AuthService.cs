using System.Net;
using System.Net.Http.Json;
using bloggy_ui.Models;
using Microsoft.JSInterop;

namespace bloggy_ui.Services;

public class AuthService {
    private static readonly string JWT_KEY = "JWT";
    private static readonly string BASE_URL = "http://localhost:5000";
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;


    public AuthService(HttpClient httpClient, IJSRuntime jSRuntime) {
        this._httpClient = httpClient;
        this._jsRuntime = jSRuntime;
    }

    public async Task<HttpStatusCode> LoginAsync(LoginRequest loginRequest) {
        try {
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"{BASE_URL}/login", loginRequest);

            if (httpResponseMessage.IsSuccessStatusCode) {
                // Read the JWT token from the response
                LoginResponse tokenResponse = await httpResponseMessage.Content.ReadFromJsonAsync<LoginResponse>();

                // Store the token in local storage.
                // Store the token in local storage
                await this.StoreJwt(tokenResponse.Token);

                string jwtretrieved = await this.RetrieveJwtValue();
                Console.WriteLine(jwtretrieved);

            } 
            return httpResponseMessage.StatusCode;
        } catch (HttpRequestException exception) {
            Console.WriteLine(exception.Message);
            return HttpStatusCode.InternalServerError;
        }
    }

    private async Task StoreJwt(string jwtValue) {
        await this._jsRuntime.InvokeVoidAsync("localStorage.setItem", JWT_KEY, jwtValue);
    }

    private async Task<string> RetrieveJwtValue() {
        return await this._jsRuntime.InvokeAsync<string>("localStorage.getItem", JWT_KEY);
    }

}