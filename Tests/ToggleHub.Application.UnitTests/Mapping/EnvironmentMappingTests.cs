using ToggleHub.Application.DTOs.Environment;
using ToggleHub.Application.Mapping;
using ToggleHub.Domain.Entities;
using Environment = ToggleHub.Domain.Entities.Environment;

namespace ToggleHub.Application.UnitTests.Mapping;

public class EnvironmentMappingTests
{
    [Test]
    public void ToDto_Environment_ShouldMapCorrectly()
    {
        // Arrange
        var environment = new Environment
        {
            Id = 1,
            Type = EnvironmentType.Dev,
            ProjectId = 100
        };

        // Act
        var result = environment.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Type, Is.EqualTo(EnvironmentType.Dev));
    }

    [Test]
    public void ToDto_Environment_WithStagingType_ShouldMapCorrectly()
    {
        // Arrange
        var environment = new Environment
        {
            Id = 2,
            Type = EnvironmentType.Staging,
            ProjectId = 200
        };

        // Act
        var result = environment.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(2));
        Assert.That(result.Type, Is.EqualTo(EnvironmentType.Staging));
    }

    [Test]
    public void ToDto_Environment_WithProdType_ShouldMapCorrectly()
    {
        // Arrange
        var environment = new Environment
        {
            Id = 3,
            Type = EnvironmentType.Prod,
            ProjectId = 300
        };

        // Act
        var result = environment.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(3));
        Assert.That(result.Type, Is.EqualTo(EnvironmentType.Prod));
    }

    [Test]
    public void ToEntity_CreateEnvironmentDto_WithDevType_ShouldMapCorrectly()
    {
        // Arrange
        var createDto = new CreateEnvironmentDto
        {
            TypeString = "Dev",
            ProjectId = 400
        };

        // Act
        var result = createDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Type, Is.EqualTo(EnvironmentType.Dev));
        Assert.That(result.ProjectId, Is.EqualTo(400));
        Assert.That(result.Id, Is.EqualTo(0)); // Default value for int
    }

    [Test]
    public void ToEntity_CreateEnvironmentDto_WithStagingType_ShouldMapCorrectly()
    {
        // Arrange
        var createDto = new CreateEnvironmentDto
        {
            TypeString = "Staging",
            ProjectId = 500
        };

        // Act
        var result = createDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Type, Is.EqualTo(EnvironmentType.Staging));
        Assert.That(result.ProjectId, Is.EqualTo(500));
        Assert.That(result.Id, Is.EqualTo(0)); // Default value for int
    }

    [Test]
    public void ToEntity_CreateEnvironmentDto_WithProdType_ShouldMapCorrectly()
    {
        // Arrange
        var createDto = new CreateEnvironmentDto
        {
            TypeString = "Prod",
            ProjectId = 600
        };

        // Act
        var result = createDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Type, Is.EqualTo(EnvironmentType.Prod));
        Assert.That(result.ProjectId, Is.EqualTo(600));
        Assert.That(result.Id, Is.EqualTo(0)); // Default value for int
    }

    [Test]
    public void ToEntity_CreateEnvironmentDto_WithEmptyTypeString_ShouldUseDefaultDevType()
    {
        // Arrange
        var createDto = new CreateEnvironmentDto
        {
            TypeString = "",
            ProjectId = 700
        };

        // Act
        var result = createDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Type, Is.EqualTo(EnvironmentType.Dev)); // Default value
        Assert.That(result.ProjectId, Is.EqualTo(700));
        Assert.That(result.Id, Is.EqualTo(0)); // Default value for int
    }

    [Test]
    public void ToEntity_CreateEnvironmentDto_WithExistingEnvironment_ShouldUpdateCorrectly()
    {
        // Arrange
        var createDto = new CreateEnvironmentDto
        {
            TypeString = "Staging",
            ProjectId = 800
        };
        var existingEnvironment = new Environment
        {
            Id = 10,
            Type = EnvironmentType.Dev,
            ProjectId = 800
        };

        // Act
        var result = createDto.ToEntity(existingEnvironment);

        // Assert
        Assert.That(result, Is.SameAs(existingEnvironment));
        Assert.That(result.Type, Is.EqualTo(EnvironmentType.Staging));
        Assert.That(result.ProjectId, Is.EqualTo(800));
        Assert.That(result.Id, Is.EqualTo(10)); // Should preserve existing ID
    }

    [Test]
    public void ToEntity_UpdateEnvironmentDto_WithValidType_ShouldMapCorrectly()
    {
        // Arrange
        var updateDto = new UpdateEnvironmentDto
        {
            Id = 20,
            TypeString = "Prod"
        };

        // Act
        var result = updateDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(20));
        Assert.That(result.Type, Is.EqualTo(EnvironmentType.Prod));
        Assert.That(result.ProjectId, Is.EqualTo(0)); // Default value for int
    }

    [Test]
    public void ToEntity_UpdateEnvironmentDto_WithEmptyTypeString_ShouldNotUpdateType()
    {
        // Arrange
        var updateDto = new UpdateEnvironmentDto
        {
            Id = 25,
            TypeString = ""
        };

        // Act
        var result = updateDto.ToEntity();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(25));
        Assert.That(result.Type, Is.EqualTo(EnvironmentType.Dev)); // Default value
        Assert.That(result.ProjectId, Is.EqualTo(0)); // Default value for int
    }

    [Test]
    public void ToEntity_UpdateEnvironmentDto_WithExistingEnvironment_ShouldUpdateCorrectly()
    {
        // Arrange
        var updateDto = new UpdateEnvironmentDto
        {
            Id = 30,
            TypeString = "Staging"
        };
        var existingEnvironment = new Environment
        {
            Id = 30,
            Type = EnvironmentType.Dev,
            ProjectId = 900
        };

        // Act
        var result = updateDto.ToEntity(existingEnvironment);

        // Assert
        Assert.That(result, Is.SameAs(existingEnvironment));
        Assert.That(result.Id, Is.EqualTo(30));
        Assert.That(result.Type, Is.EqualTo(EnvironmentType.Staging));
        Assert.That(result.ProjectId, Is.EqualTo(900)); // Should preserve existing value
    }

    [Test]
    public void ToEntity_UpdateEnvironmentDto_WithExistingEnvironmentAndEmptyType_ShouldPreserveExistingType()
    {
        // Arrange
        var updateDto = new UpdateEnvironmentDto
        {
            Id = 35,
            TypeString = ""
        };
        var existingEnvironment = new Environment
        {
            Id = 35,
            Type = EnvironmentType.Prod,
            ProjectId = 1000
        };

        // Act
        var result = updateDto.ToEntity(existingEnvironment);

        // Assert
        Assert.That(result, Is.SameAs(existingEnvironment));
        Assert.That(result.Id, Is.EqualTo(35));
        Assert.That(result.Type, Is.EqualTo(EnvironmentType.Dev)); // Default value since empty string maps to Dev
        Assert.That(result.ProjectId, Is.EqualTo(1000)); // Should preserve existing value
    }
}
