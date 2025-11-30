using StarterKit.Domain.Common;
using StarterKit.Domain.Enums;

namespace StarterKit.Domain.Entities;

public class Token : BaseEntity, IAuditableEntity
{
    public string TokenValue { get; set; } = string.Empty;
    
    public TokenType Type { get; set; }
    
    public DateTime Expires { get; set; }
    
    public bool Blacklisted { get; set; } = false;
    
    // Foreign Key
    public int UserId { get; set; }
    
    // Navigation Property
    public User? User { get; set; }
}