using ToggleHub.Application.DTOs;
using ToggleHub.Application.DTOs.OrganizationInvite;
using ToggleHub.Domain;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Mapping;

public static class OrganizationInviteMapping
{
    public static OrganizationInviteDto ToDto(this OrganizationInvite entity)
    {
        return new OrganizationInviteDto
        {
            Id = entity.Id,
            OrganizationId = entity.OrganizationId,
            OrganizationName = entity.Organization?.Name ?? string.Empty,
            Email = entity.Email,
            Token = entity.Token,
            CreatedAt = entity.CreatedAt,
            CreatedByUserId = entity.CreatedByUserId,
            ExpiresAt = entity.ExpiresAt,
            Status = entity.Status,
            AcceptedAt = entity.AcceptedAt,
            DeclinedAt = entity.DeclinedAt
        };
    }

    public static OrganizationInvite ToEntity(this CreateOrganizationInviteDto dto, string token, int createdByUserId)
    {
        return new OrganizationInvite
        {
            OrganizationId = dto.OrganizationId,
            Email = dto.Email,
            Token = token,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = createdByUserId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Status = InviteStatus.Pending
        };
    }

    public static PagedListDto<OrganizationInviteDto> ToDto(this IPagedList<OrganizationInvite> pagedList)
    {
        var dtoList = pagedList.Select(invite => invite.ToDto());
        return new PagedListDto<OrganizationInviteDto>(dtoList, pagedList.TotalCount, pagedList.PageIndex,
            pagedList.PageSize);
    }
}
