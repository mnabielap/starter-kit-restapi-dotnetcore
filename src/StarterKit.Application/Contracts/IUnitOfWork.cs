namespace StarterKit.Application.Contracts;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ITokenRepository Tokens { get; }
    Task<int> CompleteAsync();
}