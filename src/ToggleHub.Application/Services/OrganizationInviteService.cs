using FluentValidation;
using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.OrganizationInvite;
using ToggleHub.Application.Interfaces;
using ToggleHub.Application.Mapping;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Events;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class OrganizationInviteService : IOrganizationInviteService
{
    private readonly IOrganizationInviteRepository _inviteRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IOrgMemberRepository _orgMemberRepository;
    private readonly IValidator<CreateOrganizationInviteDto> _createValidator;
    private readonly IWorkContext _workContext;
    private readonly IUserService _userService;
    private readonly IEventPublisher _eventPublisher;

    public OrganizationInviteService(
        IOrganizationInviteRepository inviteRepository,
        IOrganizationRepository organizationRepository,
        IOrgMemberRepository orgMemberRepository,
        IValidator<CreateOrganizationInviteDto> createValidator,
        IWorkContext workContext, IUserService userService, IEventPublisher eventPublisher)
    {
        _inviteRepository = inviteRepository;
        _organizationRepository = organizationRepository;
        _orgMemberRepository = orgMemberRepository;
        _createValidator = createValidator;
        _workContext = workContext;
        _userService = userService;
        _eventPublisher = eventPublisher;
    }

    public async Task<OrganizationInviteDto> CreateInviteAsync(CreateOrganizationInviteDto createDto)
    {
        // Validate input
        var validationResult = await _createValidator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
            throw new ApplicationException(validationResult.Errors.First().ErrorMessage);

        // Check if organization exists
        var organization = await _organizationRepository.GetByIdAsync(createDto.OrganizationId);
        if (organization == null)
            throw new ApplicationException("Organization not found");

        // Check if user already has pending invite
        if (await _inviteRepository.HasPendingInviteAsync(createDto.Email, createDto.OrganizationId))
            throw new ApplicationException("User already has a pending invite to this organization");
        
        // Check if user is already a member of the organization
        var user = await _userService.GetUserByEmailAsync(createDto.Email);
        if (user != null && await _orgMemberRepository.IsUserInOrganizationAsync(createDto.OrganizationId, user.Id))
            throw new ApplicationException("User is already a member of this organization");

        // Create invite entity using mapping
        var currentUserId = _workContext.GetCurrentUserId();
        if(!currentUserId.HasValue)
            throw new ApplicationException("Current user is not available");
        
        var invite = createDto.ToEntity(GenerateInviteToken(), currentUserId.Value);
        invite.Organization = organization;

        await _inviteRepository.CreateAsync(invite);

        return invite.ToDto();
    }

    public async Task<OrganizationInviteDto?> GetByIdAsync(int id)
    {
        var invite = await _inviteRepository.GetByIdAsync(id);
        return invite?.ToDto();
    }

    public async Task<OrganizationInviteDto?> GetByTokenAsync(string token)
    {
        var invite = await _inviteRepository.GetByTokenAsync(token);
        return invite?.ToDto();
    }

    public async Task<PagedListDto<OrganizationInviteDto>> GetByOrganizationIdAsync(int organizationId, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var pagedInvites = await _inviteRepository.GetByOrganizationIdAsync(organizationId, pageIndex, pageSize);
        return pagedInvites.ToDto();
    }

    public async Task AcceptInviteAsync(AcceptInviteDto acceptDto)
    {
        var invite = await _inviteRepository.GetByTokenAsync(acceptDto.Token);
        
        if (invite == null)
            throw new ApplicationException("Invite not found");
        
        if (invite.Status != InviteStatus.Pending)
            throw new ApplicationException($"Invite cannot be accepted. Current status: {invite.Status}");
        
        if (invite.ExpiresAt < DateTime.UtcNow)
            throw new ApplicationException("Invite has expired");
        
        // Check if user is already a member of the organization
        if (await _orgMemberRepository.IsUserInOrganizationAsync(invite.OrganizationId, acceptDto.UserId))
            throw new ApplicationException("User is already a member of this organization");
        
        // Create organization member
        var orgMember = new OrgMember
        {
            OrganizationId = invite.OrganizationId,
            UserId = acceptDto.UserId,
            Role = OrgMemberRole.FlagManager, // Default role
        };

        await _orgMemberRepository.CreateAsync(orgMember);
        
        // Update invite status directly
        invite.Status = InviteStatus.Accepted;
        invite.AcceptedAt = DateTime.UtcNow;
        await _inviteRepository.UpdateAsync(invite);
    }

    public async Task DeclineInviteAsync(string token)
    {
        var invite = await _inviteRepository.GetByTokenAsync(token);
        
        if (invite == null)
            throw new ApplicationException("Invite not found");
        
        if (invite.Status != InviteStatus.Pending)
            throw new ApplicationException($"Invite cannot be declined. Current status: {invite.Status}");

        // Update invite status directly
        invite.Status = InviteStatus.Declined;
        invite.DeclinedAt = DateTime.UtcNow;
        await _inviteRepository.UpdateAsync(invite);
    }

    public async Task RevokeInviteAsync(int inviteId)
    {
        var invite = await _inviteRepository.GetByIdAsync(inviteId);
        
        if (invite == null)
            throw new ApplicationException("Invite not found");
        
        if (invite.Status != InviteStatus.Pending)
            throw new ApplicationException($"Invite cannot be revoked. Current status: {invite.Status}");

        // Update invite status directly
        invite.Status = InviteStatus.Revoked;
        await _inviteRepository.UpdateAsync(invite);
    }

    public async Task ResendInviteAsync(int inviteId)
    {
        var invite = await _inviteRepository.GetByIdAsync(inviteId);
        
        if (invite == null)
            throw new ApplicationException("Invite not found");
        
        if (invite.Status != InviteStatus.Pending)
            throw new ApplicationException($"Invite cannot be resent. Current status: {invite.Status}");

        // Generate new token and extend expiry
        invite.Token = GenerateInviteToken();
        invite.ExpiresAt = DateTime.UtcNow.AddDays(7);
        
        await _inviteRepository.UpdateAsync(invite);
    }

    public async Task ProcessExpiredInvitesAsync()
    {
        var expiredInvites = await _inviteRepository.GetExpiredInvitesAsync();
        
        foreach (var invite in expiredInvites)
        {
            invite.ExpiresAt = DateTime.UtcNow;
            invite.Status = InviteStatus.Expired;
        }
        await _inviteRepository.UpdateAsync(expiredInvites);
    }

    public async Task<bool> HasPendingInviteAsync(string email, int organizationId)
    {
        return await _inviteRepository.HasPendingInviteAsync(email, organizationId);
    }

    private string GenerateInviteToken()
    {
        return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
    }
}
