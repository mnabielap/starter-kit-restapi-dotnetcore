using StarterKit.Application.Contracts;
using StarterKit.Infrastructure.Data;

namespace StarterKit.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IUserRepository Users { get; }
    public ITokenRepository Tokens { get; }

    public UnitOfWork(
        ApplicationDbContext context, 
        IUserRepository userRepository, 
        ITokenRepository tokenRepository)
    {
        _context = context;
        Users = userRepository;
        Tokens = tokenRepository;
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}