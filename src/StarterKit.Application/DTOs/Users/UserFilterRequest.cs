namespace StarterKit.Application.DTOs.Users;

public class UserFilterRequest
{
    public string? Search { get; set; }
    public string? Scope { get; set; }
    public string? Name { get; set; }
    public string? Role { get; set; }
    public string? SortBy { get; set; }
    public int Limit { get; set; } = 10;
    public int Page { get; set; } = 1;
}