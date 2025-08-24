using ToggleHub.Application.DTOs.User;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.DTOs.Organization;

public class OrgMemberDto
{
    public int Id { get; set; }
    public int OrgId { get; set; }
    public OrgMemberRole Role { get; set; }
    public UserDto User { get; set; }
}