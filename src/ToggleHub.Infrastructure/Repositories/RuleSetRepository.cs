using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using ToggleHub.Infrastructure.Data;

namespace ToggleHub.Infrastructure.Repositories;

public class RuleSetRepository : BaseRepository<RuleSet>, IRuleSetRepository
{
    public RuleSetRepository(ToggleHubDbContext context) : base(context)
    {
    }
}