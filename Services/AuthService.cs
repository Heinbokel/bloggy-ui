using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Bloggy.Models;
using bloggy_ui.Models;
using Microsoft.JSInterop;

namespace bloggy_ui.Services;

/// <summary>
/// Service responsible for housing logic related to user authorization.
/// </summary>
public class AuthService {
    private static readonly string JWT_KEY = "JWT";
    private static readonly string BASE_URL = "http://localhost:5000";
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    // This subject, or observable, is responsible for providing the state of the user to the entire application.
    private BehaviorSubject<UserInformation?> userSubject = new BehaviorSubject<UserInformation?>(null);

    /// <summary>
    /// Constructor for dependency injection.
    /// </summary>
    /// <param name="httpClient">The HttpClient to provide to this class.</param>
    /// <param name="jSRuntime">The IJSRuntime to provide to this class.</param>
    public AuthService(HttpClient httpClient, IJSRuntime jSRuntime) {
        this._httpClient = httpClient;
        this._jsRuntime = jSRuntime;
    }

    /// <summary>
    /// Attempts to register the user by submitting the request to the backend.
    /// </summary>
    /// <param name="request">The UserRegisterRequest to submit.</param>
    /// <returns>The UserResponse containing the created user, or null if not successful.</returns>
    public async Task<UserResponse?> RegisterAsync(UserRegisterRequest request) {
        try {
            HttpResponseMessage httpResponseMessage = await this._httpClient.PostAsJsonAsync($"{BASE_URL}/users", request);

            if (httpResponseMessage.IsSuccessStatusCode) {                
                return await httpResponseMessage.Content.ReadFromJsonAsync<UserResponse>();
            } else {
                return null;
            }
        } catch (HttpRequestException exception) {
            Console.WriteLine(exception.ToString());
            return null;
        }
    }

    /// <summary>
    /// Attempts to log the user in, given the credentials provided in the LoginRequest.
    /// </summary>
    /// <param name="loginRequest">The LoginRequest to use, containing the credentials to be used.</param>
    /// <returns>The HttpStatusCode resulting from the HTTP Request to return.</returns>
    public async Task<HttpStatusCode> LoginAsync(LoginRequest loginRequest) {
        try {
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"{BASE_URL}/login", loginRequest);

            if (httpResponseMessage.IsSuccessStatusCode) {
                // Read the JWT token from the response
                LoginResponse tokenResponse = await httpResponseMessage.Content.ReadFromJsonAsync<LoginResponse>();

                // Store the token in local storage so any other parts of the application can retrieve it if needed.
                await this.StoreJwt(tokenResponse.Token);

                // From this point on, attach the JWT to all HTTP Requests under the Authorization Header.
                // Now all subsequent requests will have the authorization header and the JWT passed.
                // Which the backend can then validate and use as needed.
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);


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

    /// <summary>
    /// Parses the encoded JWT for user information.
    /// </summary>
    /// <param name="token">The encoded JWT to parse.</param>
    /// <returns>The Task<UserInformation> to return.</returns>
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

    /// <summary>
    /// Stores the given JWT value in local storage.
    /// </summary>
    /// <param name="jwtValue">The JWT value to store.</param>
    /// <returns>The Task to return.</returns>
    private async Task StoreJwt(string jwtValue) {
        await this._jsRuntime.InvokeVoidAsync("localStorage.setItem", JWT_KEY, jwtValue);
    }

    /// <summary>
    /// Retrieves the JWT value from local storage.
    /// </summary>
    /// <returns>The JWT value in the Task to return.</returns>
    public async Task<string> RetrieveJwtValue() {
        return await this._jsRuntime.InvokeAsync<string>("localStorage.getItem", JWT_KEY);
    }

    /// <summary>
    /// Logs the user out by setting the user subject's next value to null
    /// and removing the JWT from local storage.
    /// </summary>
    /// <returns>The Task to return.</returns>
    public async Task Logout() {
        this.userSubject.OnNext(null);
        await this._jsRuntime.InvokeVoidAsync("localStorage.removeItem", JWT_KEY);
    }

    /// <summary>
    /// Returns the user subject as an observable.
    /// </summary>
    /// <returns>The IObservable<UserInformation> to return.</returns>
    public IObservable<UserInformation> GetUserInformationAsync()
    {
        return this.userSubject.AsObservable();
    }
}