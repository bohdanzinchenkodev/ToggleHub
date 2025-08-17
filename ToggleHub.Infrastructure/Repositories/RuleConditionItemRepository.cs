using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;

namespace ToggleHub.Infrastructure.Repositories;

public class RuleConditionItemRepository : BaseRepository<RuleConditionItem>, IRuleConditionItemRepository
{
    public RuleConditionItemRepository(ToggleHubDbContext context) : base(context)
    {
    }
}