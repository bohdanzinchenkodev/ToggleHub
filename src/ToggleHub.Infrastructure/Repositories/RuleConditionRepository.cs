using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;

namespace ToggleHub.Infrastructure.Repositories;

public class RuleConditionRepository : BaseRepository<RuleCondition>, IRuleConditionRepository
{
    public RuleConditionRepository(ToggleHubDbContext context, ICacheManager cacheManager, IRepositoryCacheKeyFactory cacheKeyFactory) : base(context, cacheManager, cacheKeyFactory)
    {
    }
}