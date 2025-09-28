using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;

namespace ToggleHub.Infrastructure.Repositories;

public class RuleSetRepository : BaseRepository<RuleSet>, IRuleSetRepository
{
    public RuleSetRepository(ToggleHubDbContext context, ICacheManager cacheManager, IRepositoryCacheKeyFactory cacheKeyFactory) : base(context, cacheManager, cacheKeyFactory)
    {
    }
}