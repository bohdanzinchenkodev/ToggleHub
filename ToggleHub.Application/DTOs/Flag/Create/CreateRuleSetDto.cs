using ToggleHub.Application.DTOs.Flag;

namespace ToggleHub.Application.DTOs.Flag.Create;

public class CreateRuleSetDto : BaseRuleSetDto
{
    public IList<CreateRuleConditionDto> Conditions { get; set; } = new List<CreateRuleConditionDto>();
}