using Moq;
using ToggleHub.Application.Services;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.UnitTests.Services;

[TestFixture]
public class SlugGeneratorTests
{
    private Mock<ISluggedRepository> _mockRepository;
    private SlugGenerator _slugGenerator;

    private class TestSluggedEntity : BaseEntity, ISluggedEntity
    {
        public string Slug { get; set; } = string.Empty;
    }

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ISluggedRepository>();
        _slugGenerator = new SlugGenerator(_mockRepository.Object);
    }

    [Test]
    public async Task GenerateAsync_WithUniqueSlug_ReturnsBaseSlug()
    {
        // Arrange
        var name = "Test Organization";
        _mockRepository.Setup(r => r.GetSlugsByPatternAsync<TestSluggedEntity>("test-organization"))
            .ReturnsAsync(new List<string>());

        // Act
        var result = await _slugGenerator.GenerateAsync<TestSluggedEntity>(name);

        // Assert
        Assert.That(result, Is.EqualTo("test-organization"));
    }

    [Test]
    public async Task GenerateAsync_WithExistingSlug_ReturnsSlugWithCounter()
    {
        // Arrange
        var existingSlugs = new List<string> { "test-organization", "test-organization-1", "test-organization-3" };
        _mockRepository.Setup(r => r.GetSlugsByPatternAsync<TestSluggedEntity>("test-organization"))
            .ReturnsAsync(existingSlugs);

        // Act
        var result = await _slugGenerator.GenerateAsync<TestSluggedEntity>("Test Organization");

        // Assert
        Assert.That(result, Is.EqualTo("test-organization-4"));
    }

    [TestCase("My Organization", "my-organization")]
    [TestCase("MY ORGANIZATION", "my-organization")]
    [TestCase("My@Organization#2024!", "myorganization2024")]
    [TestCase("My    Test     Organization", "my-test-organization")]
    [TestCase("My--Test---Organization", "my-test-organization")]
    [TestCase("-My Organization-", "my-organization")]
    [TestCase("  My Organization  ", "my-organization")]
    [TestCase("Organization123 Project456", "organization123-project456")]
    public async Task GenerateAsync_WithVariousInputs_GeneratesCorrectSlug(string input, string expectedSlug)
    {
        // Arrange
        _mockRepository.Setup(r => r.GetSlugsByPatternAsync<TestSluggedEntity>(expectedSlug))
            .ReturnsAsync(new List<string>());

        // Act
        var result = await _slugGenerator.GenerateAsync<TestSluggedEntity>(input);

        // Assert
        Assert.That(result, Is.EqualTo(expectedSlug));
    }

    [Test]
    public async Task GenerateAsync_WithLongName_TruncatesTo50Characters()
    {
        // Arrange
        var longName = "This is a very long organization name that exceeds fifty characters and should be truncated";
        _mockRepository.Setup(r => r.GetSlugsByPatternAsync<TestSluggedEntity>(It.IsAny<string>()))
            .ReturnsAsync(new List<string>());

        // Act
        var result = await _slugGenerator.GenerateAsync<TestSluggedEntity>(longName);

        // Assert
        Assert.That(result.Length, Is.LessThanOrEqualTo(50));
        Assert.That(result, Does.Not.EndWith("-"));
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase("@#$%^&*()")]
    public void GenerateAsync_WithInvalidInput_ThrowsException(string invalidInput)
    {
        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => _slugGenerator.GenerateAsync<TestSluggedEntity>(invalidInput));
    }

    [Test]
    public void GenerateAsync_WithNullInput_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _slugGenerator.GenerateAsync<TestSluggedEntity>(null!));
    }
}
