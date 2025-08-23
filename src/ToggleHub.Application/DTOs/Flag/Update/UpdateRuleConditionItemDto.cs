using ToggleHub.Application.DTOs.Flag;

namespace ToggleHub.Application.DTOs.Flag.Update;

public class UpdateRuleConditionItemDto : BaseRuleConditionItemDto
{
    public int? Id { get; set; } // Nullable for new items, existing ID for updates
}
