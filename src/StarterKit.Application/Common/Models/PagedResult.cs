namespace StarterKit.Application.Common.Models;

public class PagedResult<T>
{
    public IEnumerable<T> Results { get; set; } = new List<T>();
    public int Page { get; set; }
    public int Limit { get; set; }
    public int TotalPages { get; set; }
    public int TotalResults { get; set; }
}