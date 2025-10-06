using FluentValidation;
using FluentValidation.Results;
using Moq;
using ToggleHub.Application.DTOs.Project;
using ToggleHub.Application.Interfaces;
using ToggleHub.Application.Services;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.UnitTests.Services;

public class ProjectServiceTests
{
    private Mock<IProjectRepository> _mockProjectRepository;
    private Mock<IValidator<CreateProjectDto>> _mockCreateValidator;
    private Mock<IValidator<UpdateProjectDto>> _mockUpdateValidator;
    private Mock<IOrganizationRepository> _mockOrganizationRepository;
    private Mock<ISlugGenerator> _mockSlugGenerator;
    private Mock<IEventPublisher> _mockEventPublisher;
    private ProjectService _projectService;

    [SetUp]
    public void SetUp()
    {
        
        _mockProjectRepository = new Mock<IProjectRepository>();
        _mockCreateValidator = new Mock<IValidator<CreateProjectDto>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateProjectDto>>();
        _mockOrganizationRepository = new Mock<IOrganizationRepository>();
        _mockSlugGenerator = new Mock<ISlugGenerator>();
        _mockEventPublisher = new Mock<IEventPublisher>();

        _projectService = new ProjectService(
            _mockProjectRepository.Object,
            _mockCreateValidator.Object,
            _mockSlugGenerator.Object,
            _mockUpdateValidator.Object,
            _mockOrganizationRepository.Object,
            _mockEventPublisher.Object);
    }

    [Test]
    public async Task CreateAsync_WithValidData_ShouldCreateProject()
    {
        // Arrange
        var createDto = new CreateProjectDto
        {
            OrganizationId = 1,
            Name = "Test Project"
        };

        var validationResult = new Mock<ValidationResult>();
        validationResult.Setup(vr => vr.IsValid).Returns(true);
        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, CancellationToken.None))
            .ReturnsAsync(validationResult.Object);

        var organization = new Organization
        {
            Id = 1,
            Name = "Test Organization",
            Slug = "test-org"
        };

        _mockOrganizationRepository.Setup(r => r.GetByIdAsync(createDto.OrganizationId))
            .ReturnsAsync(organization);

        _mockProjectRepository.Setup(r => r.NameExistsAsync(createDto.Name, organization.Id))
            .ReturnsAsync(false);

        var expectedSlug = "test-project";
        _mockSlugGenerator.Setup(s => s.GenerateAsync(createDto.Name, It.IsAny<Func<string, Task<IEnumerable<string>>>>()))
            .ReturnsAsync(expectedSlug);

        var createdProject = new Project
        {
            Id = 1,
            OrganizationId = 1,
            Name = createDto.Name,
            Slug = expectedSlug,
            CreatedAt = DateTime.UtcNow
        };

        _mockProjectRepository.Setup(r => r.CreateAsync(It.IsAny<Project>(), true))
            .ReturnsAsync(createdProject);

        // Act
        var result = await _projectService.CreateAsync(createDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.OrganizationId, Is.EqualTo(1));
        Assert.That(result.Name, Is.EqualTo("Test Project"));
        Assert.That(result.Slug, Is.EqualTo(expectedSlug));

        _mockOrganizationRepository.Verify(r => r.GetByIdAsync(createDto.OrganizationId), Times.Once);
        _mockProjectRepository.Verify(r => r.NameExistsAsync(createDto.Name, organization.Id), Times.Once);
        _mockSlugGenerator.Verify(s => s.GenerateAsync(createDto.Name, It.IsAny<Func<string, Task<IEnumerable<string>>>>()), Times.Once);
        _mockProjectRepository.Verify(r => r.CreateAsync(It.IsAny<Project>(), true), Times.Once);
    }

    [Test]
    public void CreateAsync_WithInvalidValidation_ShouldThrowValidationException()
    {
        // Arrange
        var createDto = new CreateProjectDto
        {
            OrganizationId = 1,
            Name = "Invalid Project"
        };

        var validationResult = new ValidationResult([new ValidationFailure("Name", "Name is required")]);

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, CancellationToken.None))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ValidationException>(() => 
            _projectService.CreateAsync(createDto));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Errors, Is.Not.Empty);
        Assert.That(exception.Errors.First().PropertyName, Is.EqualTo("Name"));
        Assert.That(exception.Errors.First().ErrorMessage, Is.EqualTo("Name is required"));

        _mockOrganizationRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockProjectRepository.Verify(r => r.NameExistsAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        _mockSlugGenerator.Verify(s => s.GenerateAsync(It.IsAny<string>(), null), Times.Never);
        _mockProjectRepository.Verify(r => r.CreateAsync(It.IsAny<Project>(), true), Times.Never);
    }

    [Test]
    public void CreateAsync_WithNonExistentOrganization_ShouldThrowApplicationException()
    {
        // Arrange
        var createDto = new CreateProjectDto
        {
            OrganizationId = 999,
            Name = "Test Project"
        };

        var validationResult = new Mock<ValidationResult>();
        validationResult.Setup(vr => vr.IsValid).Returns(true);
        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, CancellationToken.None))
            .ReturnsAsync(validationResult.Object);

        _mockOrganizationRepository.Setup(r => r.GetByIdAsync(createDto.OrganizationId))
            .ReturnsAsync((Organization?)null);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ApplicationException>(() => 
            _projectService.CreateAsync(createDto));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Is.EqualTo("Organization with ID 999 not found"));

        _mockProjectRepository.Verify(r => r.NameExistsAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        _mockSlugGenerator.Verify(s => s.GenerateAsync(It.IsAny<string>(), null), Times.Never);
        _mockProjectRepository.Verify(r => r.CreateAsync(It.IsAny<Project>(), true), Times.Never);
    }

    [Test]
    public void CreateAsync_WithExistingNameInOrganization_ShouldThrowApplicationException()
    {
        // Arrange
        var createDto = new CreateProjectDto
        {
            OrganizationId = 1,
            Name = "Existing Project"
        };

        var validationResult = new Mock<ValidationResult>();
        validationResult.Setup(vr => vr.IsValid).Returns(true);
        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, CancellationToken.None))
            .ReturnsAsync(validationResult.Object);

        var organization = new Organization
        {
            Id = 1,
            Name = "Test Organization",
            Slug = "test-org"
        };

        _mockOrganizationRepository.Setup(r => r.GetByIdAsync(createDto.OrganizationId))
            .ReturnsAsync(organization);

        _mockProjectRepository.Setup(r => r.NameExistsAsync(createDto.Name, organization.Id))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ApplicationException>(() => 
            _projectService.CreateAsync(createDto));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Is.EqualTo("Project with name 'Existing Project' already exists in your organization."));

        _mockSlugGenerator.Verify(s => s.GenerateAsync(It.IsAny<string>(), null), Times.Never);
        _mockProjectRepository.Verify(r => r.CreateAsync(It.IsAny<Project>(), true), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_WithValidData_ShouldUpdateProject()
    {
        // Arrange
        var updateDto = new UpdateProjectDto
        {
            Id = 1,
            Name = "Updated Project"
        };

        var validationResult = new Mock<ValidationResult>();
        validationResult.Setup(vr => vr.IsValid).Returns(true);
        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, CancellationToken.None))
            .ReturnsAsync(validationResult.Object);

        var existingProject = new Project
        {
            Id = 1,
            OrganizationId = 1,
            Name = "Original Project",
            Slug = "original-project",
            CreatedAt = DateTime.UtcNow
        };

        _mockProjectRepository.Setup(r => r.GetByIdAsync(updateDto.Id))
            .ReturnsAsync(existingProject);

        var organization = new Organization
        {
            Id = 1,
            Name = "Test Organization",
            Slug = "test-org"
        };

        _mockOrganizationRepository.Setup(r => r.GetByIdAsync(existingProject.OrganizationId))
            .ReturnsAsync(organization);

        _mockProjectRepository.Setup(r => r.NameExistsAsync(updateDto.Name, organization.Id))
            .ReturnsAsync(false);

        var newSlug = "updated-project";
        _mockSlugGenerator.Setup(s => s.GenerateAsync(updateDto.Name, It.IsAny<Func<string, Task<IEnumerable<string>>>>()))
            .ReturnsAsync(newSlug);

        // Act
        var result = await _projectService.UpdateAsync(updateDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Name, Is.EqualTo("Updated Project"));
        Assert.That(result.Slug, Is.EqualTo(newSlug));

        _mockProjectRepository.Verify(r => r.GetByIdAsync(updateDto.Id), Times.Once);
        _mockOrganizationRepository.Verify(r => r.GetByIdAsync(existingProject.OrganizationId), Times.Once);
        _mockProjectRepository.Verify(r => r.NameExistsAsync(updateDto.Name, organization.Id), Times.Once);
        _mockSlugGenerator.Verify(s => s.GenerateAsync(updateDto.Name, It.IsAny<Func<string, Task<IEnumerable<string>>>>()), Times.Once);
        _mockProjectRepository.Verify(r => r.UpdateAsync(It.Is<Project>(p => 
            p.Id == 1 && 
            p.Name == "Updated Project" && 
            p.Slug == newSlug), true), Times.Once);
    }

    [Test]
    public void UpdateAsync_WithInvalidValidation_ShouldThrowValidationException()
    {
        // Arrange
        var updateDto = new UpdateProjectDto
        {
            Id = 1,
            Name = "Invalid Project"
        };

        var validationResult = new ValidationResult([new ValidationFailure("Name", "Name is required")]);

        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, CancellationToken.None))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ValidationException>(() => 
            _projectService.UpdateAsync(updateDto));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Errors, Is.Not.Empty);
        Assert.That(exception.Errors.First().PropertyName, Is.EqualTo("Name"));
        Assert.That(exception.Errors.First().ErrorMessage, Is.EqualTo("Name is required"));

        _mockProjectRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockOrganizationRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockProjectRepository.Verify(r => r.NameExistsAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        _mockSlugGenerator.Verify(s => s.GenerateAsync(It.IsAny<string>(), null), Times.Never);
        _mockProjectRepository.Verify(r => r.UpdateAsync(It.IsAny<Project>(), true), Times.Never);
    }

    [Test]
    public void UpdateAsync_WithNonExistentId_ShouldThrowApplicationException()
    {
        // Arrange
        var updateDto = new UpdateProjectDto
        {
            Id = 999,
            Name = "Non-existent Project"
        };

        var validationResult = new Mock<ValidationResult>();
        validationResult.Setup(vr => vr.IsValid).Returns(true);
        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, CancellationToken.None))
            .ReturnsAsync(validationResult.Object);

        _mockProjectRepository.Setup(r => r.GetByIdAsync(updateDto.Id))
            .ReturnsAsync((Project?)null);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ApplicationException>(() => 
            _projectService.UpdateAsync(updateDto));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Is.EqualTo("Project with ID 999 not found"));

        _mockOrganizationRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockProjectRepository.Verify(r => r.NameExistsAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        _mockSlugGenerator.Verify(s => s.GenerateAsync(It.IsAny<string>(), null), Times.Never);
        _mockProjectRepository.Verify(r => r.UpdateAsync(It.IsAny<Project>(), true), Times.Never);
    }

    [Test]
    public void UpdateAsync_WithExistingNameInOrganization_ShouldThrowApplicationException()
    {
        // Arrange
        var updateDto = new UpdateProjectDto
        {
            Id = 1,
            Name = "Existing Project"
        };

        var validationResult = new Mock<ValidationResult>();
        validationResult.Setup(vr => vr.IsValid).Returns(true);
        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, CancellationToken.None))
            .ReturnsAsync(validationResult.Object);

        var existingProject = new Project
        {
            Id = 1,
            OrganizationId = 1,
            Name = "Original Project",
            Slug = "original-project",
            CreatedAt = DateTime.UtcNow
        };

        _mockProjectRepository.Setup(r => r.GetByIdAsync(updateDto.Id))
            .ReturnsAsync(existingProject);

        var organization = new Organization
        {
            Id = 1,
            Name = "Test Organization",
            Slug = "test-org"
        };

        _mockOrganizationRepository.Setup(r => r.GetByIdAsync(existingProject.OrganizationId))
            .ReturnsAsync(organization);

        _mockProjectRepository.Setup(r => r.NameExistsAsync(updateDto.Name, organization.Id))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ApplicationException>(() => 
            _projectService.UpdateAsync(updateDto));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Is.EqualTo("Project with name 'Existing Project' already exists."));

        _mockSlugGenerator.Verify(s => s.GenerateAsync(It.IsAny<string>(), null), Times.Never);
        _mockProjectRepository.Verify(r => r.UpdateAsync(It.IsAny<Project>(), true), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_WithSameName_ShouldNotGenerateNewSlug()
    {
        // Arrange
        var updateDto = new UpdateProjectDto
        {
            Id = 1,
            Name = "Same Project"
        };

        var validationResult = new Mock<ValidationResult>();
        validationResult.Setup(vr => vr.IsValid).Returns(true);
        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, CancellationToken.None))
            .ReturnsAsync(validationResult.Object);

        var existingProject = new Project
        {
            Id = 1,
            OrganizationId = 1,
            Name = "Same Project",
            Slug = "same-project",
            CreatedAt = DateTime.UtcNow
        };

        _mockProjectRepository.Setup(r => r.GetByIdAsync(updateDto.Id))
            .ReturnsAsync(existingProject);

        var organization = new Organization
        {
            Id = 1,
            Name = "Test Organization",
            Slug = "test-org"
        };

        _mockOrganizationRepository.Setup(r => r.GetByIdAsync(existingProject.OrganizationId))
            .ReturnsAsync(organization);

        // Act
        var result = await _projectService.UpdateAsync(updateDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Same Project"));
        Assert.That(result.Slug, Is.EqualTo("same-project"));

        _mockProjectRepository.Verify(r => r.NameExistsAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        _mockSlugGenerator.Verify(s => s.GenerateAsync(It.IsAny<string>(), null), Times.Never);
        _mockProjectRepository.Verify(r => r.UpdateAsync(It.Is<Project>(p => 
            p.Id == 1 && 
            p.Name == "Same Project" && 
            p.Slug == "same-project"), true), Times.Once);
    }
}
