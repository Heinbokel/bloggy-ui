using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Bloggy.Models;
using bloggy_ui.Models;
using Microsoft.JSInterop;

namespace bloggy_ui.Services;

public class AuthService {
    private static readonly string JWT_KEY = "JWT";
    private static readonly string BASE_URL = "http://localhost:5000";
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    private BehaviorSubject<UserInformation?> userSubject = new BehaviorSubject<UserInformation?>(null);

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

                // Store the token in local storage so any other parts of the application can retrieve it if needed.
                await this.StoreJwt(tokenResponse.Token);

                // Parse the JWT and store the claims in our User subject in this class.
                // This will also notify all observers that the user information has been updated.
                UserInformation userInformation = await this.ParseJwtForUser(tokenResponse.Token);
                this.userSubject.OnNext(userInformation);
            } 
            return httpResponseMessage.StatusCode;
        } catch (HttpRequestException exception) {
            Console.WriteLine(exception.Message);
            return HttpStatusCode.InternalServerError;
        }
    }

    private async Task<UserInformation> ParseJwtForUser(string token)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        JwtSecurityToken jwtSecurityToken = tokenHandler.ReadJwtToken(token);

        // Extract custom claims from the token
        string firstNameClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "firstName")?.Value;
        string lastNameClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "lastName")?.Value;
        string emailClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
        string dateOfBirthClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "dateOfBirth")?.Value;
        string idClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        string userRoleIdClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "userRoleId")?.Value;
        string userNameClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "userName")?.Value;

        return new UserInformation {
            FirstName = firstNameClaim,
            LastName = lastNameClaim,
            Email = emailClaim,
            DateOfBirth = dateOfBirthClaim,
            Id = idClaim,
            UserName = userNameClaim,
            UserRoleId = userRoleIdClaim
        };
    }

    private async Task StoreJwt(string jwtValue) {
        await this._jsRuntime.InvokeVoidAsync("localStorage.setItem", JWT_KEY, jwtValue);
    }

    private async Task<string> RetrieveJwtValue() {
        return await this._jsRuntime.InvokeAsync<string>("localStorage.getItem", JWT_KEY);
    }

    public IObservable<UserInformation> GetUserInformationAsync()
    {
        return this.userSubject.AsObservable();
    }
}