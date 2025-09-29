using FluentValidation;
using FluentValidation.Results;
using Moq;
using ToggleHub.Application.DTOs.Organization;
using ToggleHub.Application.Interfaces;
using ToggleHub.Application.Services;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.UnitTests.Services;

public class OrganizationServiceTests
{
    private Mock<IOrganizationRepository> _mockOrganizationRepository;
    private Mock<IValidator<CreateOrganizationDto>> _mockCreateValidator;
    private Mock<IValidator<UpdateOrganizationDto>> _mockUpdateValidator;
    private Mock<ISlugGenerator> _mockSlugGenerator;
    private Mock<IWorkContext> _mockWorkContext;
    private OrganizationService _organizationService;
    

    [SetUp]
    public void SetUp()
    {
        _mockOrganizationRepository = new Mock<IOrganizationRepository>();
        _mockCreateValidator = new Mock<IValidator<CreateOrganizationDto>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateOrganizationDto>>();
        _mockSlugGenerator = new Mock<ISlugGenerator>();
        var mockOrganizationPermissionService = new Mock<IOrganizationPermissionService>();
        _mockWorkContext = new Mock<IWorkContext>();
        var mockOrgMemberRepository = new Mock<IOrgMemberRepository>();

        _organizationService = new OrganizationService(
            _mockOrganizationRepository.Object,
            _mockCreateValidator.Object,
            _mockSlugGenerator.Object,
            _mockUpdateValidator.Object,
            mockOrganizationPermissionService.Object,
            _mockWorkContext.Object,
            mockOrgMemberRepository.Object);
    }

    [Test]
    public async Task CreateAsync_WithValidData_ShouldCreateOrganization()
    {
        // Arrange
        var createDto = new CreateOrganizationDto
        {
            Name = "Test Organization"
        };

        var validationResult = new Mock<ValidationResult>();
        validationResult.Setup(vr => vr.IsValid).Returns(true);
        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default))
            .ReturnsAsync(validationResult.Object);

        _mockOrganizationRepository.Setup(r => r.NameExistsAsync(createDto.Name))
            .ReturnsAsync(false);

        var expectedSlug = "test-organization";
        _mockSlugGenerator.Setup(s => s.GenerateAsync(createDto.Name, It.IsAny<Func<string, Task<IEnumerable<string>>>>()))
            .ReturnsAsync(expectedSlug);

        var createdOrganization = new Organization
        {
            Id = 1,
            Name = createDto.Name,
            Slug = expectedSlug,
            CreatedAt = DateTime.UtcNow
        };

        _mockOrganizationRepository.Setup(r => r.CreateAsync(It.IsAny<Organization>(), true))
            .ReturnsAsync(createdOrganization);

        _mockWorkContext.Setup(wc => wc.GetCurrentUserId())
            .Returns(42); // Simulate a logged-in user with ID 42
        // Act
        var result = await _organizationService.CreateAsync(createDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Name, Is.EqualTo("Test Organization"));
        Assert.That(result.Slug, Is.EqualTo(expectedSlug));

        _mockOrganizationRepository.Verify(r => r.NameExistsAsync(createDto.Name), Times.Once);
        _mockSlugGenerator.Verify(s => s.GenerateAsync(createDto.Name, It.IsAny<Func<string, Task<IEnumerable<string>>>>()), Times.Once);
        _mockOrganizationRepository.Verify(r => r.CreateAsync(It.IsAny<Organization>(), true), Times.Once);
    }

    [Test]
    public async Task CreateAsync_WithInvalidValidation_ShouldThrowValidationException()
    {
        // Arrange
        var createDto = new CreateOrganizationDto
        {
            Name = "Invalid Organization"
        };

        var validationResult = new ValidationResult([new ValidationFailure("Name", "Name is required")]);

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ValidationException>(() => 
            _organizationService.CreateAsync(createDto));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Errors, Is.Not.Empty);
        Assert.That(exception.Errors.First().PropertyName, Is.EqualTo("Name"));
        Assert.That(exception.Errors.First().ErrorMessage, Is.EqualTo("Name is required"));

        _mockOrganizationRepository.Verify(r => r.NameExistsAsync(It.IsAny<string>()), Times.Never);
        _mockSlugGenerator.Verify(s => s.GenerateAsync(createDto.Name, It.IsAny<Func<string, Task<IEnumerable<string>>>>()), Times.Never);
        _mockOrganizationRepository.Verify(r => r.CreateAsync(It.IsAny<Organization>(), true), Times.Never);
    }

    [Test]
    public async Task CreateAsync_WithExistingName_ShouldThrowApplicationException()
    {
        // Arrange
        var createDto = new CreateOrganizationDto
        {
            Name = "Existing Organization"
        };

        var validationResult = new Mock<ValidationResult>();
        validationResult.Setup(vr => vr.IsValid).Returns(true);
        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, CancellationToken.None))
            .ReturnsAsync(validationResult.Object);

        _mockOrganizationRepository.Setup(r => r.NameExistsAsync(createDto.Name))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ApplicationException>(() => 
            _organizationService.CreateAsync(createDto));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Is.EqualTo("Organization with name Existing Organization already exists"));

        _mockSlugGenerator.Verify(s => s.GenerateAsync(It.IsAny<string>(), null), Times.Never);
        _mockOrganizationRepository.Verify(r => r.CreateAsync(It.IsAny<Organization>(), true), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_WithValidData_ShouldUpdateOrganization()
    {
        // Arrange
        var updateDto = new UpdateOrganizationDto
        {
            Id = 1,
            Name = "Updated Organization"
        };

        var validationResult = new Mock<ValidationResult>();
        validationResult.Setup(vr => vr.IsValid).Returns(true);
        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, CancellationToken.None))
            .ReturnsAsync(validationResult.Object);

        var existingOrganization = new Organization
        {
            Id = 1,
            Name = "Original Organization",
            Slug = "original-organization",
            CreatedAt = DateTime.UtcNow
        };

        _mockOrganizationRepository.Setup(r => r.GetByIdAsync(updateDto.Id))
            .ReturnsAsync(existingOrganization);

        _mockOrganizationRepository.Setup(r => r.NameExistsAsync(updateDto.Name))
            .ReturnsAsync(false);

        var newSlug = "updated-organization";
        _mockSlugGenerator.Setup(s => s.GenerateAsync(updateDto.Name, It.IsAny<Func<string, Task<IEnumerable<string>>>>()))
            .ReturnsAsync(newSlug);

        // Act
        await _organizationService.UpdateAsync(updateDto);

        // Assert
        _mockOrganizationRepository.Verify(r => r.GetByIdAsync(updateDto.Id), Times.Once);
        _mockOrganizationRepository.Verify(r => r.NameExistsAsync(updateDto.Name), Times.Once);
        _mockSlugGenerator.Verify(s => s.GenerateAsync(updateDto.Name, It.IsAny<Func<string, Task<IEnumerable<string>>>>()), Times.Once);
        _mockOrganizationRepository.Verify(r => r.UpdateAsync(It.Is<Organization>(o => 
            o.Id == 1 && 
            o.Name == "Updated Organization" && 
            o.Slug == newSlug), true), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WithInvalidValidation_ShouldThrowValidationException()
    {
        // Arrange
        var updateDto = new UpdateOrganizationDto
        {
            Id = 1,
            Name = "Invalid Organization"
        };

        var validationResult = new ValidationResult([new ValidationFailure("Name", "Name is required")]);

        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, CancellationToken.None))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ValidationException>(() => 
            _organizationService.UpdateAsync(updateDto));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Errors, Is.Not.Empty);
        Assert.That(exception.Errors.First().PropertyName, Is.EqualTo("Name"));
        Assert.That(exception.Errors.First().ErrorMessage, Is.EqualTo("Name is required"));

        _mockOrganizationRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockOrganizationRepository.Verify(r => r.NameExistsAsync(It.IsAny<string>()), Times.Never);
        _mockSlugGenerator.Verify(s => s.GenerateAsync(It.IsAny<string>(), It.IsAny<Func<string, Task<IEnumerable<string>>>>()), Times.Never);
        _mockOrganizationRepository.Verify(r => r.UpdateAsync(It.IsAny<Organization>(), true), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_WithNonExistentId_ShouldThrowApplicationException()
    {
        // Arrange
        var updateDto = new UpdateOrganizationDto
        {
            Id = 999,
            Name = "Non-existent Organization"
        };

        var validationResult = new Mock<ValidationResult>();
        validationResult.Setup(vr => vr.IsValid).Returns(true);
        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, CancellationToken.None))
            .ReturnsAsync(validationResult.Object);

        _mockOrganizationRepository.Setup(r => r.GetByIdAsync(updateDto.Id))
            .ReturnsAsync((Organization?)null);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ApplicationException>(() => 
            _organizationService.UpdateAsync(updateDto));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Is.EqualTo("Organization with ID 999 not found"));

        _mockOrganizationRepository.Verify(r => r.NameExistsAsync(It.IsAny<string>()), Times.Never);
        _mockSlugGenerator.Verify(s => s.GenerateAsync(It.IsAny<string>(), null), Times.Never);
        _mockOrganizationRepository.Verify(r => r.UpdateAsync(It.IsAny<Organization>(), true), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_WithExistingName_ShouldThrowApplicationException()
    {
        // Arrange
        var updateDto = new UpdateOrganizationDto
        {
            Id = 1,
            Name = "Existing Organization"
        };

        var validationResult = new Mock<ValidationResult>();
        validationResult.Setup(vr => vr.IsValid).Returns(true);
        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, CancellationToken.None))
            .ReturnsAsync(validationResult.Object);

        var existingOrganization = new Organization
        {
            Id = 1,
            Name = "Original Organization",
            Slug = "original-organization",
            CreatedAt = DateTime.UtcNow
        };

        _mockOrganizationRepository.Setup(r => r.GetByIdAsync(updateDto.Id))
            .ReturnsAsync(existingOrganization);

        _mockOrganizationRepository.Setup(r => r.NameExistsAsync(updateDto.Name))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ApplicationException>(() => 
            _organizationService.UpdateAsync(updateDto));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Is.EqualTo("Organization with name Existing Organization already exists"));

        _mockSlugGenerator.Verify(s => s.GenerateAsync(It.IsAny<string>(), It.IsAny<Func<string, Task<IEnumerable<string>>>>()), Times.Never);
        _mockOrganizationRepository.Verify(r => r.UpdateAsync(It.IsAny<Organization>(), true), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_WithSameName_ShouldNotGenerateNewSlug()
    {
        // Arrange
        var updateDto = new UpdateOrganizationDto
        {
            Id = 1,
            Name = "Same Organization"
        };

        var validationResult = new Mock<ValidationResult>();
        validationResult.Setup(vr => vr.IsValid).Returns(true);
        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, CancellationToken.None))
            .ReturnsAsync(validationResult.Object);

        var existingOrganization = new Organization
        {
            Id = 1,
            Name = "Same Organization",
            Slug = "same-organization",
            CreatedAt = DateTime.UtcNow
        };

        _mockOrganizationRepository.Setup(r => r.GetByIdAsync(updateDto.Id))
            .ReturnsAsync(existingOrganization);

        // Act
        await _organizationService.UpdateAsync(updateDto);

        // Assert
        _mockOrganizationRepository.Verify(r => r.GetByIdAsync(updateDto.Id), Times.Once);
        _mockOrganizationRepository.Verify(r => r.NameExistsAsync(It.IsAny<string>()), Times.Never);
        _mockSlugGenerator.Verify(s => s.GenerateAsync(It.IsAny<string>(), It.IsAny<Func<string, Task<IEnumerable<string>>>>()), Times.Never);
        _mockOrganizationRepository.Verify(r => r.UpdateAsync(It.Is<Organization>(o => 
            o.Id == 1 && 
            o.Name == "Same Organization" && 
            o.Slug == "same-organization"), true), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WithDifferentName_ShouldGenerateNewSlug()
    {
        // Arrange
        var updateDto = new UpdateOrganizationDto
        {
            Id = 1,
            Name = "Completely Different Name"
        };

        var validationResult = new Mock<ValidationResult>();
        validationResult.Setup(vr => vr.IsValid).Returns(true);
        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, CancellationToken.None))
            .ReturnsAsync(validationResult.Object);

        var existingOrganization = new Organization
        {
            Id = 1,
            Name = "Original Organization",
            Slug = "original-organization",
            CreatedAt = DateTime.UtcNow
        };

        _mockOrganizationRepository.Setup(r => r.GetByIdAsync(updateDto.Id))
            .ReturnsAsync(existingOrganization);

        _mockOrganizationRepository.Setup(r => r.NameExistsAsync(updateDto.Name))
            .ReturnsAsync(false);

        var newSlug = "completely-different-name";
        _mockSlugGenerator.Setup(s => s.GenerateAsync(updateDto.Name, It.IsAny<Func<string, Task<IEnumerable<string>>>>()))
            .ReturnsAsync(newSlug);

        // Act
        await _organizationService.UpdateAsync(updateDto);

        // Assert
        _mockOrganizationRepository.Verify(r => r.GetByIdAsync(updateDto.Id), Times.Once);
        _mockOrganizationRepository.Verify(r => r.NameExistsAsync(updateDto.Name), Times.Once);
        _mockSlugGenerator.Verify(s => s.GenerateAsync(updateDto.Name, It.IsAny<Func<string, Task<IEnumerable<string>>>>()), Times.Once);
        _mockOrganizationRepository.Verify(r => r.UpdateAsync(It.Is<Organization>(o => 
            o.Id == 1 && 
            o.Name == "Completely Different Name" && 
            o.Slug == newSlug), true), Times.Once);
    }
}
