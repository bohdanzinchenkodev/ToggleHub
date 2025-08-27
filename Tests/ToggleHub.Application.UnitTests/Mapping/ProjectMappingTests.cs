using ToggleHub.Application.DTOs.Project;
using ToggleHub.Application.Mapping;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.UnitTests.Mapping;

public class ProjectMappingTests
{
    [Test]
    public void ToDto_Project_ShouldMapCorrectly()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            OrganizationId = 100,
            Name = "Test Project",
            Slug = "test-project",
            CreatedAt = new DateTime(2024, 1, 1)
        };

        // Act
        var result = project.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.OrganizationId, Is.EqualTo(100));
        Assert.That(result.Name, Is.EqualTo("Test Project"));
        Assert.That(result.Slug, Is.EqualTo("test-project"));
        Assert.That(result.CreatedAt, Is.EqualTo(new DateTime(2024, 1, 1)));
    }

    [Test]
    public void ToEntity_CreateProjectDto_ShouldMapCorrectly()
    {
        // Arrange
        var createDto = new CreateProjectDto
        {
            OrganizationId = 200,
            Name = "New Project"
        };

        // Act
        var result = createDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.OrganizationId, Is.EqualTo(200));
        Assert.That(result.Name, Is.EqualTo("New Project"));
        Assert.That(result.Id, Is.EqualTo(0)); // Default value for int
        Assert.That(result.Slug, Is.EqualTo(string.Empty)); // Default value for string
    }

    [Test]
    public void ToEntity_CreateProjectDto_WithExistingProject_ShouldUpdateCorrectly()
    {
        // Arrange
        var createDto = new CreateProjectDto
        {
            OrganizationId = 300,
            Name = "Updated Project"
        };
        var existingProject = new Project
        {
            Id = 5,
            OrganizationId = 300,
            Name = "Old Name",
            Slug = "old-slug",
            CreatedAt = new DateTime(2024, 1, 1)
        };

        // Act
        var result = createDto.ToEntity(existingProject);

        // Assert
        Assert.That(result, Is.SameAs(existingProject));
        Assert.That(result.OrganizationId, Is.EqualTo(300));
        Assert.That(result.Name, Is.EqualTo("Updated Project"));
        Assert.That(result.Id, Is.EqualTo(5)); // Should preserve existing ID
        Assert.That(result.Slug, Is.EqualTo("old-slug")); // Should preserve existing slug
        Assert.That(result.CreatedAt, Is.EqualTo(new DateTime(2024, 1, 1))); // Should preserve existing date
    }

    [Test]
    public void ToEntity_UpdateProjectDto_ShouldMapCorrectly()
    {
        // Arrange
        var updateDto = new UpdateProjectDto
        {
            Id = 10,
            Name = "Updated Project Name"
        };

        // Act
        var result = updateDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(10));
        Assert.That(result.Name, Is.EqualTo("Updated Project Name"));
        Assert.That(result.OrganizationId, Is.EqualTo(0)); // Default value for int
        Assert.That(result.Slug, Is.EqualTo(string.Empty)); // Default value for string
    }

    [Test]
    public void ToEntity_UpdateProjectDto_WithExistingProject_ShouldUpdateCorrectly()
    {
        // Arrange
        var updateDto = new UpdateProjectDto
        {
            Id = 15,
            Name = "Completely Updated Project"
        };
        var existingProject = new Project
        {
            Id = 15,
            OrganizationId = 400,
            Name = "Original Name",
            Slug = "original-slug",
            CreatedAt = new DateTime(2024, 1, 1)
        };

        // Act
        var result = updateDto.ToEntity(existingProject);

        // Assert
        Assert.That(result, Is.SameAs(existingProject));
        Assert.That(result.Id, Is.EqualTo(15));
        Assert.That(result.Name, Is.EqualTo("Completely Updated Project"));
        Assert.That(result.OrganizationId, Is.EqualTo(400)); // Should preserve existing value
        Assert.That(result.Slug, Is.EqualTo("original-slug")); // Should preserve existing slug
        Assert.That(result.CreatedAt, Is.EqualTo(new DateTime(2024, 1, 1))); // Should preserve existing date
    }
}
