namespace Bloggy.Models;

/// <summary>
/// Represents a blog post, created by a given user.
/// </summary>
public class BlogPostResponse {

    public int Id { get; set;}
    public string Title { get; set;}
    public string Content { get; set;}
    public DateOnly DatePosted { get; set;}
    public int UserId { get; set;}

}