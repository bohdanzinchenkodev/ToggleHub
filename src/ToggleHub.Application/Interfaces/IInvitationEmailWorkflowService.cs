using ToggleHub.Application.DTOs.OrganizationInvite;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Interfaces;

public interface IInvitationEmailWorkflowService
{
    Task SendInvitationEmailAsync(OrganizationInvite invite);
}