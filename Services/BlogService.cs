using System.Net.Http.Json;
using System.Reactive.Linq;
using Bloggy.Models;

namespace bloggy_ui.Services;

/// <summary>
/// Manages all functionalities and states surrounding blog posts.
/// </summary>
public class BlogService {

    private string BASE_URL;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Constructor for dependency injection.
    /// </summary>
    /// <param name="httpClient">The HttpClient to provide to this class.</param>
    /// <param name="configuration">The IConfiguration to provide this class.</param>
    public BlogService(HttpClient httpClient, IConfiguration configuration) {
        this._httpClient = httpClient;
        this._configuration = configuration;
        this.BASE_URL = _configuration.GetValue<string>("BaseUrl");
    }

    /// <summary>
    /// Returns the async GetBlogPosts method result as an observable.
    /// </summary>
    /// <returns>The IObservable<BlogPostResponse[]> to return.</returns>
    public IObservable<BlogPostResponse[]> GetBlogPostsAsObservable() {
        return Observable.FromAsync(GetBlogPosts);
    }

    /// <summary>
    /// Retrieves the list of blog posts from the backend.
    /// </summary>
    /// <returns>The Task<BlogPostResponse[]> to return.</returns>
    public async Task<BlogPostResponse[]> GetBlogPosts() {
        try {
            HttpResponseMessage httpResponseMessage = await this._httpClient.GetAsync($"{BASE_URL}/blog-posts");

            if (httpResponseMessage.IsSuccessStatusCode) {
                return await httpResponseMessage.Content.ReadFromJsonAsync<BlogPostResponse[]>();
            } else {
                return [];
            }
        } catch (HttpRequestException exception) {
            // probably would handle this by displaying an error message somewhere.
            // for now we can just log the exception to the console and return an empty collection.
            Console.WriteLine("An error occurred calling for blog posts.", exception);
            return [];
        }
    }

}