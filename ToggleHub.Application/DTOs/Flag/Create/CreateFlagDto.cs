using ToggleHub.Application.DTOs.Flag;

namespace ToggleHub.Application.DTOs.Flag.Create;

public class CreateFlagDto : BaseFlagDto
{
    public IList<CreateRuleSetDto> RuleSets { get; set; } = new List<CreateRuleSetDto>();
}