using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;

namespace ToggleHub.Infrastructure.Repositories;

public class RuleConditionItemRepository : BaseRepository<RuleConditionItem>, IRuleConditionItemRepository
{
    public RuleConditionItemRepository(ToggleHubDbContext context, ICacheManager cacheManager, ICacheKeyFactory cacheKeyFactory, IEventPublisher eventPublisher) : base(context, cacheManager, cacheKeyFactory, eventPublisher)
    {
    }
}