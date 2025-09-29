using FluentValidation;
using FluentValidation.Results;
using Moq;
using ToggleHub.Application.DTOs.Environment;
using ToggleHub.Application.Interfaces;
using ToggleHub.Application.Services;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;
using Environment = ToggleHub.Domain.Entities.Environment;

namespace ToggleHub.Application.UnitTests.Services;

public class EnvironmentServiceTests
{
    private Mock<IEnvironmentRepository> _mockEnvironmentRepository;
    private Mock<IValidator<CreateEnvironmentDto>> _mockCreateValidator;
    private Mock<IValidator<UpdateEnvironmentDto>> _mockUpdateValidator;
    private Mock<IEventPublisher> _mockEventPublisher;
    private EnvironmentService _environmentService;
    
    [SetUp]
    public void SetUp()
    {
        _mockEnvironmentRepository = new Mock<IEnvironmentRepository>();
        _mockCreateValidator = new Mock<IValidator<CreateEnvironmentDto>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateEnvironmentDto>>();
        _mockEventPublisher = new Mock<IEventPublisher>();
        

        _environmentService = new EnvironmentService(
            _mockEnvironmentRepository.Object,
            _mockCreateValidator.Object,
            _mockUpdateValidator.Object,
            _mockEventPublisher.Object);
    }

    [Test]
    public async Task CreateAsync_WithValidData_ShouldCreateEnvironment()
    {
        // Arrange
        var createDto = new CreateEnvironmentDto
        {
            TypeString = "Dev",
            ProjectId = 1
        };

        var validationResult = new Mock<ValidationResult>();
        validationResult.Setup(vr => vr.IsValid).Returns(true);
        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, CancellationToken.None))
            .ReturnsAsync(validationResult.Object);

        var createdEnvironment = new Environment
        {
            Id = 1,
            Type = EnvironmentType.Dev,
            ProjectId = 1
        };

        _mockEnvironmentRepository.Setup(r => r.CreateAsync(It.IsAny<Environment>(), true))
            .ReturnsAsync(createdEnvironment);

        // Act
        var result = await _environmentService.CreateAsync(createDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Type, Is.EqualTo(EnvironmentType.Dev));

        _mockEnvironmentRepository.Verify(r => r.CreateAsync(It.IsAny<Environment>(), true), Times.Once);
    }

    [Test]
    public async Task CreateAsync_WithInvalidValidation_ShouldThrowValidationException()
    {
        // Arrange
        var createDto = new CreateEnvironmentDto
        {
            TypeString = "Invalid",
            ProjectId = 1
        };

        var validationResult = new ValidationResult([new ValidationFailure("TypeString", "Invalid environment type")]);

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, CancellationToken.None))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ValidationException>(() => 
            _environmentService.CreateAsync(createDto));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Errors, Is.Not.Empty);
        Assert.That(exception.Errors.First().PropertyName, Is.EqualTo("TypeString"));
        Assert.That(exception.Errors.First().ErrorMessage, Is.EqualTo("Invalid environment type"));

        _mockEnvironmentRepository.Verify(r => r.CreateAsync(It.IsAny<Environment>(), true), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_WithValidData_ShouldUpdateEnvironment()
    {
        // Arrange
        var updateDto = new UpdateEnvironmentDto
        {
            Id = 1,
            TypeString = "Staging"
        };

        var validationResult = new Mock<ValidationResult>();
        validationResult.Setup(vr => vr.IsValid).Returns(true);
        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, CancellationToken.None))
            .ReturnsAsync(validationResult.Object);

        var existingEnvironment = new Environment
        {
            Id = 1,
            Type = EnvironmentType.Dev,
            ProjectId = 1
        };

        _mockEnvironmentRepository.Setup(r => r.GetByIdAsync(updateDto.Id))
            .ReturnsAsync(existingEnvironment);

        // Act
        var result = await _environmentService.UpdateAsync(updateDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Type, Is.EqualTo(EnvironmentType.Staging));

        _mockEnvironmentRepository.Verify(r => r.GetByIdAsync(updateDto.Id), Times.Once);
        _mockEnvironmentRepository.Verify(r => r.UpdateAsync(It.Is<Environment>(e => 
            e.Id == 1 && 
            e.Type == EnvironmentType.Staging), true), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WithInvalidValidation_ShouldThrowValidationException()
    {
        // Arrange
        var updateDto = new UpdateEnvironmentDto
        {
            Id = 1,
            TypeString = "Invalid"
        };

        var validationResult = new ValidationResult([new ValidationFailure("TypeString", "Invalid environment type")]);

        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, CancellationToken.None))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ValidationException>(() => 
            _environmentService.UpdateAsync(updateDto));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Errors, Is.Not.Empty);
        Assert.That(exception.Errors.First().PropertyName, Is.EqualTo("TypeString"));
        Assert.That(exception.Errors.First().ErrorMessage, Is.EqualTo("Invalid environment type"));

        _mockEnvironmentRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockEnvironmentRepository.Verify(r => r.UpdateAsync(It.IsAny<Environment>(), true), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_WithNonExistentId_ShouldThrowApplicationException()
    {
        // Arrange
        var updateDto = new UpdateEnvironmentDto
        {
            Id = 999,
            TypeString = "Prod"
        };

        var validationResult = new Mock<ValidationResult>();
        validationResult.Setup(vr => vr.IsValid).Returns(true);
        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, CancellationToken.None))
            .ReturnsAsync(validationResult.Object);

        _mockEnvironmentRepository.Setup(r => r.GetByIdAsync(updateDto.Id))
            .ReturnsAsync((Environment?)null);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ApplicationException>(() => 
            _environmentService.UpdateAsync(updateDto));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Is.EqualTo("Environment with id 999 not found."));

        _mockEnvironmentRepository.Verify(r => r.UpdateAsync(It.IsAny<Environment>(), true), Times.Never);
    }

    [Test]
    public async Task DeleteAsync_WithNonExistentId_ShouldThrowApplicationException()
    {
        // Arrange
        var id = 999;

        _mockEnvironmentRepository.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Environment?)null);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ApplicationException>(() => 
            _environmentService.DeleteAsync(id));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception.Message, Is.EqualTo("Environment with id 999 not found."));

        _mockEnvironmentRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), true), Times.Never);
    }

    [Test]
    public async Task DeleteAsync_WithValidId_ShouldDeleteEnvironment()
    {
        // Arrange
        var id = 1;
        var environment = new Environment
        {
            Id = 1,
            Type = EnvironmentType.Dev,
            ProjectId = 1
        };

        _mockEnvironmentRepository.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(environment);

        // Act
        await _environmentService.DeleteAsync(id);

        // Assert
        _mockEnvironmentRepository.Verify(r => r.GetByIdAsync(id), Times.Once);
        _mockEnvironmentRepository.Verify(r => r.DeleteAsync(id, true), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_WithValidId_ShouldReturnEnvironment()
    {
        // Arrange
        var id = 1;
        var environment = new Environment
        {
            Id = 1,
            Type = EnvironmentType.Prod,
            ProjectId = 1
        };

        _mockEnvironmentRepository.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(environment);

        // Act
        var result = await _environmentService.GetByIdAsync(id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Type, Is.EqualTo(EnvironmentType.Prod));

        _mockEnvironmentRepository.Verify(r => r.GetByIdAsync(id), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var id = 999;

        _mockEnvironmentRepository.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Environment?)null);

        // Act
        var result = await _environmentService.GetByIdAsync(id);

        // Assert
        Assert.That(result, Is.Null);

        _mockEnvironmentRepository.Verify(r => r.GetByIdAsync(id), Times.Once);
    }
    
}
