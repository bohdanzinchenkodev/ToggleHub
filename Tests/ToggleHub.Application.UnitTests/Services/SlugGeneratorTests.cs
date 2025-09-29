using ToggleHub.Application.Services;

namespace ToggleHub.Application.UnitTests.Services;

public class SlugGeneratorTests
{
    private SlugGenerator _slugGenerator;

    [SetUp]
    public void SetUp()
    {
        _slugGenerator = new SlugGenerator();
    }

    [TestCase("Hello World", "hello-world")]
    [TestCase("Hello    World", "hello-world")]
    [TestCase("Hello-World", "hello-world")]
    [TestCase("UPPERCASE", "uppercase")]
    [TestCase("Mixed Case Text", "mixed-case-text")]
    [TestCase("Text with 123 numbers", "text-with-123-numbers")]
    [TestCase("Special!@#$%^&*()Characters", "specialcharacters")]
    [TestCase("Multiple---Hyphens", "multiple-hyphens")]
    [TestCase("   Leading and trailing spaces   ", "leading-and-trailing-spaces")]
    [TestCase("-Leading-hyphen", "leading-hyphen")]
    [TestCase("Trailing-hyphen-", "trailing-hyphen")]
    [TestCase("unicode-café-naïve", "unicode-caf-nave")]
    public async Task GenerateAsync_WithVariousInputs_ShouldGenerateCorrectSlug(string input, string expected)
    {
        // Act
        var result = await _slugGenerator.GenerateAsync(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public async Task GenerateAsync_WithVeryLongString_ShouldTruncateTo50Characters()
    {
        // Arrange
        var longString = "this-is-a-very-long-string-that-should-be-truncated-because-it-exceeds-the-maximum-allowed-length-for-slugs";

        // Act
        var result = await _slugGenerator.GenerateAsync(longString);

        // Assert
        Assert.That(result.Length, Is.LessThanOrEqualTo(50));
        Assert.That(result, Does.Not.EndWith("-"));
    }

    [Test]
    public void GenerateAsync_WithOnlySpecialCharacters_ShouldThrowException()
    {
        // Arrange
        var specialCharsOnly = "!@#$%^&*()";

        // Act & Assert
        var ex = Assert.ThrowsAsync<ApplicationException>(
            () => _slugGenerator.GenerateAsync(specialCharsOnly));
        
        Assert.That(ex.Message, Is.EqualTo("Generated slug is empty. Please provide a valid name."));
    }

    [TestCase("")]
    [TestCase("   ")]
    public void GenerateAsync_WithEmptyString_ShouldThrowApplicationException(string input)
    {
        // Act & Assert
        var ex = Assert.ThrowsAsync<ApplicationException>(
            () => _slugGenerator.GenerateAsync(input));
        
        Assert.That(ex.Message, Is.EqualTo("Name cannot be null or empty"));
    }

    [Test]
    public void GenerateAsync_WithNullString_ShouldThrowApplicationException()
    {
        // Act & Assert
        var ex = Assert.ThrowsAsync<ApplicationException>(
            () => _slugGenerator.GenerateAsync(null!));
        
        Assert.That(ex.Message, Is.EqualTo("Name cannot be null or empty"));
    }

    [Test]
    public async Task GenerateAsync_WithNoExistingSlugs_ShouldReturnBaseSlug()
    {
        // Arrange
        var name = "My Project";
        
        // Act
        var result = await _slugGenerator.GenerateAsync(name);

        // Assert
        Assert.That(result, Is.EqualTo("my-project"));
    }

    [Test]
    public async Task GenerateAsync_WithExistingSlugFactoryReturningEmpty_ShouldReturnBaseSlug()
    {
        // Arrange
        var name = "My Project";
        Func<string, Task<IEnumerable<string>>> existingSlugsFactory = _ => Task.FromResult(Enumerable.Empty<string>());

        // Act
        var result = await _slugGenerator.GenerateAsync(name, existingSlugsFactory);

        // Assert
        Assert.That(result, Is.EqualTo("my-project"));
    }

    [Test]
    public async Task GenerateAsync_WithBaseSlugExists_ShouldReturnSlugWithCounter()
    {
        // Arrange
        var name = "My Project";
        var existingSlugs = new[] { "my-project" };
        Func<string, Task<IEnumerable<string>>> existingSlugsFactory = _ => Task.FromResult(existingSlugs.AsEnumerable());

        // Act
        var result = await _slugGenerator.GenerateAsync(name, existingSlugsFactory);

        // Assert
        Assert.That(result, Is.EqualTo("my-project-1"));
    }

    [Test]
    public async Task GenerateAsync_WithMultipleExistingSlugs_ShouldReturnNextAvailableCounter()
    {
        // Arrange
        var name = "My Project";
        var existingSlugs = new[] { "my-project", "my-project-1", "my-project-3", "my-project-5" };
        Func<string, Task<IEnumerable<string>>> existingSlugsFactory = _ => Task.FromResult(existingSlugs.AsEnumerable());

        // Act
        var result = await _slugGenerator.GenerateAsync(name, existingSlugsFactory);

        // Assert
        Assert.That(result, Is.EqualTo("my-project-6"));
    }

    [Test]
    public async Task GenerateAsync_WithNonConsecutiveCounters_ShouldReturnHighestPlusOne()
    {
        // Arrange
        var name = "Test";
        var existingSlugs = new[] { "test", "test-2", "test-10", "test-5" };
        Func<string, Task<IEnumerable<string>>> existingSlugsFactory = _ => Task.FromResult(existingSlugs.AsEnumerable());

        // Act
        var result = await _slugGenerator.GenerateAsync(name, existingSlugsFactory);

        // Assert
        Assert.That(result, Is.EqualTo("test-11"));
    }

    [Test]
    public async Task GenerateAsync_WithSimilarButDifferentSlugs_ShouldIgnoreNonMatching()
    {
        // Arrange
        var name = "Test";
        var existingSlugs = new[] { "test", "testing", "test-project", "test-1", "test-abc" };
        Func<string, Task<IEnumerable<string>>> existingSlugsFactory = _ => Task.FromResult(existingSlugs.AsEnumerable());

        // Act
        var result = await _slugGenerator.GenerateAsync(name, existingSlugsFactory);

        // Assert
        Assert.That(result, Is.EqualTo("test-2"));
    }

    [Test]
    public async Task GenerateAsync_WithInvalidCounterFormats_ShouldIgnoreInvalidCounters()
    {
        // Arrange
        var name = "Project";
        var existingSlugs = new[] { "project", "project-1", "project-abc", "project-1a", "project-", "project-2" };
        Func<string, Task<IEnumerable<string>>> existingSlugsFactory = _ => Task.FromResult(existingSlugs.AsEnumerable());

        // Act
        var result = await _slugGenerator.GenerateAsync(name, existingSlugsFactory);

        // Assert
        Assert.That(result, Is.EqualTo("project-3"));
    }

    [Test]
    public async Task GenerateAsync_WithGapsInCounters_ShouldStillReturnHighestPlusOne()
    {
        // Arrange
        var name = "Item";
        var existingSlugs = new[] { "item", "item-1", "item-3", "item-7" };
        Func<string, Task<IEnumerable<string>>> existingSlugsFactory = _ => Task.FromResult(existingSlugs.AsEnumerable());

        // Act
        var result = await _slugGenerator.GenerateAsync(name, existingSlugsFactory);

        // Assert
        Assert.That(result, Is.EqualTo("item-8"));
    }

    [Test]
    public async Task GenerateAsync_WithLargeCounters_ShouldHandleLargeNumbers()
    {
        // Arrange
        var name = "Big";
        var existingSlugs = new[] { "big", "big-999", "big-1000" };
        Func<string, Task<IEnumerable<string>>> existingSlugsFactory = _ => Task.FromResult(existingSlugs.AsEnumerable());

        // Act
        var result = await _slugGenerator.GenerateAsync(name, existingSlugsFactory);

        // Assert
        Assert.That(result, Is.EqualTo("big-1001"));
    }

    [Test]
    public async Task GenerateAsync_WithZeroCounter_ShouldHandleZeroCorrectly()
    {
        // Arrange
        var name = "Zero";
        var existingSlugs = new[] { "zero", "zero-0", "zero-1" };
        Func<string, Task<IEnumerable<string>>> existingSlugsFactory = _ => Task.FromResult(existingSlugs.AsEnumerable());

        // Act
        var result = await _slugGenerator.GenerateAsync(name, existingSlugsFactory);

        // Assert
        Assert.That(result, Is.EqualTo("zero-2"));
    }

    [Test]
    public void GenerateAsync_WithFactoryThrowingException_ShouldPropagateException()
    {
        // Arrange
        var name = "Error Test";
        Func<string, Task<IEnumerable<string>>> faultyFactory = _ => throw new InvalidOperationException("Database error");

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(
            () => _slugGenerator.GenerateAsync(name, faultyFactory));
        
        Assert.That(ex.Message, Is.EqualTo("Database error"));
    }

    [Test]
    public async Task GenerateAsync_WithAsyncFactory_ShouldWorkWithAsyncOperations()
    {
        // Arrange
        var name = "Async Test";
        var existingSlugs = new[] { "async-test" };
        Func<string, Task<IEnumerable<string>>> asyncFactory = async baseSlug =>
        {
            await Task.Delay(1); // Simulate async work
            return existingSlugs.AsEnumerable();
        };

        // Act
        var result = await _slugGenerator.GenerateAsync(name, asyncFactory);

        // Assert
        Assert.That(result, Is.EqualTo("async-test-1"));
    }

    [TestCase("test", new[] { "test" }, "test-1")]
    [TestCase("test", new[] { "test", "test-1" }, "test-2")]
    [TestCase("test", new[] { "test", "test-1", "test-2" }, "test-3")]
    [TestCase("test", new string[0], "test")]
    [TestCase("project", new[] { "project-5" }, "project")]
    public async Task GenerateAsync_WithVariousExistingSlugCombinations_ShouldReturnCorrectSlug(
        string name, 
        string[] existingSlugs, 
        string expected)
    {
        // Arrange
        Func<string, Task<IEnumerable<string>>> existingSlugsFactory = _ => Task.FromResult(existingSlugs.AsEnumerable());

        // Act
        var result = await _slugGenerator.GenerateAsync(name, existingSlugsFactory);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
}
