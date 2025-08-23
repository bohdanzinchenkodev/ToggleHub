using ToggleHub.Application.DTOs.Flag;

namespace ToggleHub.Application.DTOs.Flag.Update;

public class UpdateRuleSetDto : BaseRuleSetDto
{
    public int? Id { get; set; } // Nullable for new items, existing ID for updates
    public IList<UpdateRuleConditionDto> Conditions { get; set; } = new List<UpdateRuleConditionDto>();
}
