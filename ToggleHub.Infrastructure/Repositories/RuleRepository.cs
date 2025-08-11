using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;

namespace ToggleHub.Infrastructure.Repositories;

public class RuleRepository : BaseRepository<Rule>, IRuleRepository
{
    public RuleRepository(ToggleHubDbContext context) : base(context)
    {
    }
}
