using ToggleHub.Application.DTOs.Flag;

namespace ToggleHub.Application.DTOs.Flag.Update;

public class UpdateRuleConditionDto : BaseRuleConditionDto
{
    public int? Id { get; set; } // Nullable for new items, existing ID for updates
    public IList<UpdateRuleConditionItemDto> Items { get; set; } = new List<UpdateRuleConditionItemDto>();
}
