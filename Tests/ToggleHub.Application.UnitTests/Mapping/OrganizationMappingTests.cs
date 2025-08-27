using ToggleHub.Application.DTOs.Organization;
using ToggleHub.Application.DTOs.User;
using ToggleHub.Application.Mapping;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.UnitTests.Mapping;

public class OrganizationMappingTests
{
    [Test]
    public void ToDto_Organization_ShouldMapCorrectly()
    {
        // Arrange
        var organization = new Organization
        {
            Id = 1,
            Name = "Test Organization",
            Slug = "test-org",
            CreatedAt = new DateTime(2024, 1, 1)
        };

        // Act
        var result = organization.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Name, Is.EqualTo("Test Organization"));
        Assert.That(result.Slug, Is.EqualTo("test-org"));
    }

    [Test]
    public void ToEntity_CreateOrganizationDto_ShouldMapCorrectly()
    {
        // Arrange
        var createDto = new CreateOrganizationDto
        {
            Name = "New Organization"
        };

        // Act
        var result = createDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("New Organization"));
        Assert.That(result.Id, Is.EqualTo(0)); // Default value for int
        Assert.That(result.Slug, Is.EqualTo(string.Empty)); // Default value for string
    }

    [Test]
    public void ToEntity_CreateOrganizationDto_WithExistingOrganization_ShouldUpdateCorrectly()
    {
        // Arrange
        var createDto = new CreateOrganizationDto
        {
            Name = "Updated Organization"
        };
        var existingOrganization = new Organization
        {
            Id = 1,
            Name = "Old Name",
            Slug = "old-slug",
            CreatedAt = new DateTime(2024, 1, 1)
        };

        // Act
        var result = createDto.ToEntity(existingOrganization);

        // Assert
        Assert.That(result, Is.SameAs(existingOrganization));
        Assert.That(result.Name, Is.EqualTo("Updated Organization"));
        Assert.That(result.Id, Is.EqualTo(1)); // Should preserve existing ID
        Assert.That(result.Slug, Is.EqualTo("old-slug")); // Should preserve existing slug
        Assert.That(result.CreatedAt, Is.EqualTo(new DateTime(2024, 1, 1))); // Should preserve existing date
    }

    [Test]
    public void ToEntity_UpdateOrganizationDto_ShouldMapCorrectly()
    {
        // Arrange
        var updateDto = new UpdateOrganizationDto
        {
            Id = 2,
            Name = "Updated Organization"
        };

        // Act
        var result = updateDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(2));
        Assert.That(result.Name, Is.EqualTo("Updated Organization"));
        Assert.That(result.Slug, Is.EqualTo(string.Empty)); // Default value for string
    }

    [Test]
    public void ToEntity_UpdateOrganizationDto_WithExistingOrganization_ShouldUpdateCorrectly()
    {
        // Arrange
        var updateDto = new UpdateOrganizationDto
        {
            Id = 3,
            Name = "Completely Updated Organization"
        };
        var existingOrganization = new Organization
        {
            Id = 3,
            Name = "Original Name",
            Slug = "original-slug",
            CreatedAt = new DateTime(2024, 1, 1)
        };

        // Act
        var result = updateDto.ToEntity(existingOrganization);

        // Assert
        Assert.That(result, Is.SameAs(existingOrganization));
        Assert.That(result.Id, Is.EqualTo(3));
        Assert.That(result.Name, Is.EqualTo("Completely Updated Organization"));
        Assert.That(result.Slug, Is.EqualTo("original-slug")); // Should preserve existing slug
        Assert.That(result.CreatedAt, Is.EqualTo(new DateTime(2024, 1, 1))); // Should preserve existing date
    }

    [Test]
    public void ToDto_OrgMember_ShouldMapCorrectly()
    {
        // Arrange
        var orgMember = new OrgMember
        {
            Id = 1,
            OrganizationId = 100,
            UserId = 200,
            Role = OrgMemberRole.Admin
        };

        var userDto = new UserDto
        {
            Id = 200,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Roles = new List<UserRoleDto>()
        };

        // Act
        var result = orgMember.ToDto(userDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.OrganizationId, Is.EqualTo(100));
        Assert.That(result.OrganizationRole, Is.EqualTo(OrgMemberRole.Admin));
        Assert.That(result.User, Is.SameAs(userDto));
    }

    [Test]
    public void ToDto_OrgMember_WithOwnerRole_ShouldMapCorrectly()
    {
        // Arrange
        var orgMember = new OrgMember
        {
            Id = 2,
            OrganizationId = 101,
            UserId = 201,
            Role = OrgMemberRole.Owner
        };

        var userDto = new UserDto
        {
            Id = 201,
            Email = "owner@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            Roles = new List<UserRoleDto>()
        };

        // Act
        var result = orgMember.ToDto(userDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(2));
        Assert.That(result.OrganizationId, Is.EqualTo(101));
        Assert.That(result.OrganizationRole, Is.EqualTo(OrgMemberRole.Owner));
        Assert.That(result.User, Is.SameAs(userDto));
    }

    [Test]
    public void ToDto_OrgMember_WithFlagManagerRole_ShouldMapCorrectly()
    {
        // Arrange
        var orgMember = new OrgMember
        {
            Id = 3,
            OrganizationId = 102,
            UserId = 202,
            Role = OrgMemberRole.FlagManager
        };

        var userDto = new UserDto
        {
            Id = 202,
            Email = "flagmanager@example.com",
            FirstName = "Bob",
            LastName = "Johnson",
            Roles = new List<UserRoleDto>()
        };

        // Act
        var result = orgMember.ToDto(userDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(3));
        Assert.That(result.OrganizationId, Is.EqualTo(102));
        Assert.That(result.OrganizationRole, Is.EqualTo(OrgMemberRole.FlagManager));
        Assert.That(result.User, Is.SameAs(userDto));
    }

    [Test]
    public void ToDto_OrgMember_WithUserRoles_ShouldMapCorrectly()
    {
        // Arrange
        var orgMember = new OrgMember
        {
            Id = 4,
            OrganizationId = 103,
            UserId = 203,
            Role = OrgMemberRole.Admin
        };

        var userRoles = new List<UserRoleDto>
        {
            new UserRoleDto { Id = 1, Role = "User" },
            new UserRoleDto { Id = 2, Role = "Premium" }
        };

        var userDto = new UserDto
        {
            Id = 203,
            Email = "admin@example.com",
            FirstName = "Alice",
            LastName = "Brown",
            Roles = userRoles
        };

        // Act
        var result = orgMember.ToDto(userDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(4));
        Assert.That(result.OrganizationId, Is.EqualTo(103));
        Assert.That(result.OrganizationRole, Is.EqualTo(OrgMemberRole.Admin));
        Assert.That(result.User, Is.SameAs(userDto));
        Assert.That(result.User.Roles.Count, Is.EqualTo(2));
        Assert.That(result.User.Roles.Any(ur => ur.Role == "User"), Is.True);
        Assert.That(result.User.Roles.Any(ur => ur.Role == "Premium"), Is.True);
    }
}
