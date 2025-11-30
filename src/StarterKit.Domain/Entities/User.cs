using StarterKit.Domain.Common;
using StarterKit.Domain.Enums;

namespace StarterKit.Domain.Entities;

public class User : BaseEntity, IAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
    
    public Role Role { get; set; } = Role.User;
    
    public bool IsEmailVerified { get; set; } = false;

    // Navigation Property
    public ICollection<Token> Tokens { get; set; } = new List<Token>();
}