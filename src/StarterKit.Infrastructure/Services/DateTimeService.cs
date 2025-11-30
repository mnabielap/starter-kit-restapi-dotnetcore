using StarterKit.Application.Common.Interfaces;

namespace StarterKit.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.UtcNow;
}