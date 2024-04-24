using System.Net.Http.Json;
using System.Reactive.Linq;
using Bloggy.Models;

namespace bloggy_ui.Services;

public class BlogService {

    private static readonly string BASE_URL = "http://localhost:5000";
    private readonly HttpClient _httpClient;
    public BlogService(HttpClient httpClient) {
        this._httpClient = httpClient;
    }

    public IObservable<BlogPostResponse[]> GetBlogPostsAsObservable() {
        return Observable.FromAsync(GetBlogPosts);
    }

    public async Task<BlogPostResponse[]> GetBlogPosts() {
        HttpResponseMessage httpResponseMessage = await this._httpClient.GetAsync($"{BASE_URL}/blog-posts");

        if (httpResponseMessage.IsSuccessStatusCode) {
            return await httpResponseMessage.Content.ReadFromJsonAsync<BlogPostResponse[]>();
        } else {
            return [];
        }
    }

}