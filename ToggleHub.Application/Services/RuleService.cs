using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class RuleService : BaseService<Rule>
{
    private readonly IRuleRepository _ruleRepository;

    public RuleService(IRuleRepository ruleRepository) 
        : base(ruleRepository)
    {
        _ruleRepository = ruleRepository;
    }
}
