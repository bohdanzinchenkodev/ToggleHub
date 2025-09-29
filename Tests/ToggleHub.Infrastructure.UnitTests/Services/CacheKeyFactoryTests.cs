using Moq;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Infrastructure.Cache;
using ToggleHub.Infrastructure.Settings;

namespace ToggleHub.Infrastructure.UnitTests.Services;

public class CacheKeyFactoryTests
{
    private Mock<ICacheKeyFormatter> _mockFormatter;
    private CacheSettings _cacheSettings;
    private CacheKeyFactory _cacheKeyFactory;

    [SetUp]
    public void SetUp()
    {
        _mockFormatter = new Mock<ICacheKeyFormatter>();
        _cacheSettings = new CacheSettings { DefaultCacheTimeMinutes = 10 };
        _cacheKeyFactory = new CacheKeyFactory(_mockFormatter.Object, _cacheSettings);

        // Setup default formatter behavior
        _mockFormatter.Setup(f => f.Format(It.IsAny<string>(), It.IsAny<object[]>()))
            .Returns<string, object[]>((template, args) => string.Format(template, args));
    }

    [Test]
    public void For_WithEntityNameAndParameters_ShouldCreateCorrectCacheKey()
    {
        // Arrange
        var entityName = "organization";
        var parameters = new Dictionary<string, object?> { { "id", 123 }, { "name", "test" } };

        // Act
        var result = _cacheKeyFactory.For(entityName, parameters);

        // Assert
        Assert.That(result.Key, Is.EqualTo("entity:organization:id=123:name=test"));
        Assert.That(result.CacheTime, Is.EqualTo(10));
    }

    [Test]
    public void For_WithOrderedParameters_ShouldMaintainStableOrder()
    {
        // Arrange
        var entityName = "test";
        var parameters = new Dictionary<string, object?> 
        { 
            { "zebra", "last" }, 
            { "alpha", "first" }, 
            { "beta", "second" } 
        };

        // Act
        var result = _cacheKeyFactory.For(entityName, parameters);

        // Assert - Should be ordered alphabetically by key
        Assert.That(result.Key, Is.EqualTo("entity:test:alpha=first:beta=second:zebra=last"));
    }

    [Test]
    public void For_WithNullValues_ShouldHandleNullsCorrectly()
    {
        // Arrange
        var entityName = "test";
        var parameters = new Dictionary<string, object?> { { "id", 123 }, { "nullable", null } };

        // Act
        var result = _cacheKeyFactory.For(entityName, parameters);

        // Assert
        Assert.That(result.Key, Is.EqualTo("entity:test:id=123:nullable=null"));
    }

    [Test]
    public void For_Generic_ShouldUseEntityTypeName()
    {
        // Arrange
        var parameters = new Dictionary<string, object?> { { "id", 456 } };

        // Act
        var result = _cacheKeyFactory.For<Organization>(parameters);

        // Assert
        Assert.That(result.Key, Is.EqualTo("entity:organization:id=456"));
        Assert.That(result.CacheTime, Is.EqualTo(10));
    }

    [Test]
    public void ForEntityById_ShouldCreateIdBasedKey()
    {
        // Arrange
        var id = 789;

        // Act
        var result = _cacheKeyFactory.ForEntityById<Project>(id);

        // Assert
        Assert.That(result.Key, Is.EqualTo("entity:project:id=789"));
        Assert.That(result.CacheTime, Is.EqualTo(10));
    }

    [Test]
    public void ForEntityAll_ShouldCreatePaginationKey()
    {
        // Arrange
        var page = 2;
        var pageSize = 50;

        // Act
        var result = _cacheKeyFactory.ForEntityAll<Organization>(page, pageSize);

        // Assert
        Assert.That(result.Key, Is.EqualTo("entity:organization:page=2:pageSize=50"));
        Assert.That(result.CacheTime, Is.EqualTo(10));
    }

    [Test]
    public void ForSlug_ShouldCreateSlugBasedKey()
    {
        // Arrange
        var slug = "my-organization";

        // Act
        var result = _cacheKeyFactory.ForSlug<Organization>(slug);

        // Assert
        Assert.That(result.Key, Is.EqualTo("entity:organization:slug=my-organization"));
        Assert.That(result.CacheTime, Is.EqualTo(10));
    }

    [Test]
    public void PrefixForEntity_ShouldReturnCorrectPrefix()
    {
        // Act
        var result = _cacheKeyFactory.PrefixForEntity<Project>();

        // Assert
        Assert.That(result, Is.EqualTo("entity:project:"));
    }

    [Test]
    public void For_WithEmptyParameters_ShouldCreateSimpleKey()
    {
        // Arrange
        var parameters = new Dictionary<string, object?>();

        // Act
        var result = _cacheKeyFactory.For("simple", parameters);

        // Assert
        Assert.That(result.Key, Is.EqualTo("entity:simple"));
        Assert.That(result.CacheTime, Is.EqualTo(10));
    }

    [Test]
    public void For_WithDifferentCacheSettings_ShouldUseCacheTime()
    {
        // Arrange
        var customSettings = new CacheSettings { DefaultCacheTimeMinutes = 30 };
        var customFactory = new CacheKeyFactory(_mockFormatter.Object, customSettings);
        var parameters = new Dictionary<string, object?> { { "id", 1 } };

        // Act
        var result = customFactory.For<Organization>(parameters);

        // Assert
        Assert.That(result.CacheTime, Is.EqualTo(30));
    }

    [Test]
    public void For_WithSameParametersInDifferentOrder_ShouldProduceSameKey()
    {
        // Arrange
        var entityName = "test";
        var parameters1 = new Dictionary<string, object?> { { "b", 2 }, { "a", 1 } };
        var parameters2 = new Dictionary<string, object?> { { "a", 1 }, { "b", 2 } };

        // Act
        var result1 = _cacheKeyFactory.For(entityName, parameters1);
        var result2 = _cacheKeyFactory.For(entityName, parameters2);

        // Assert
        Assert.That(result1.Key, Is.EqualTo(result2.Key));
    }

    [Test]
    public void PrefixForEntity_WithDifferentEntities_ShouldReturnDifferentPrefixes()
    {
        // Act
        var orgPrefix = _cacheKeyFactory.PrefixForEntity<Organization>();
        var projectPrefix = _cacheKeyFactory.PrefixForEntity<Project>();

        // Assert
        Assert.That(orgPrefix, Is.EqualTo("entity:organization:"));
        Assert.That(projectPrefix, Is.EqualTo("entity:project:"));
        Assert.That(orgPrefix, Is.Not.EqualTo(projectPrefix));
    }

}
