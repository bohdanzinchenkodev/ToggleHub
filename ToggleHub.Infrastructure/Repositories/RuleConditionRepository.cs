using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;

namespace ToggleHub.Infrastructure.Repositories;

public class RuleConditionRepository : BaseRepository<RuleCondition>, IRuleConditionRepository
{
    public RuleConditionRepository(ToggleHubDbContext context) : base(context)
    {
    }
}