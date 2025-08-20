using ToggleHub.Application.DTOs.Flag;

namespace ToggleHub.Application.DTOs.Flag.Create;

public class CreateRuleConditionDto : BaseRuleConditionDto
{
    public IList<CreateRuleConditionItemDto> Items { get; set; } = new List<CreateRuleConditionItemDto>();
}