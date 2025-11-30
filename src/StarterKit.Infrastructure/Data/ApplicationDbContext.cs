using System.Reflection;
using Microsoft.EntityFrameworkCore;
using StarterKit.Domain.Entities;
using StarterKit.Infrastructure.Data.Interceptors;

namespace StarterKit.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    private readonly AuditableEntityInterceptor _auditableEntityInterceptor;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, AuditableEntityInterceptor auditableEntityInterceptor) 
        : base(options)
    {
        _auditableEntityInterceptor = auditableEntityInterceptor;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Token> Tokens => Set<Token>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntityInterceptor);
    }
}