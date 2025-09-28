using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;

namespace ToggleHub.Infrastructure.Repositories;

public class AuditLogRepository : BaseRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(ToggleHubDbContext context, ICacheManager cacheManager, ICacheKeyFactory cacheKeyFactory) : base(context, cacheManager, cacheKeyFactory)
    {
    }
}
