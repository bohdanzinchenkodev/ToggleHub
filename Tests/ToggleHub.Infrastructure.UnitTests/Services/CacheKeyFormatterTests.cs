using System.Globalization;
using ToggleHub.Domain.Entities;
using ToggleHub.Infrastructure.Cache;

namespace ToggleHub.Infrastructure.UnitTests.Services;

public class CacheKeyFormatterTests
{
    private CacheKeyFormatter _formatter;

    [SetUp]
    public void SetUp()
    {
        _formatter = new CacheKeyFormatter();
    }

    [Test]
    public void Format_WithSimpleParameters_ShouldFormatCorrectly()
    {
        // Act
        var result = _formatter.Format("entity:{0}:id={1}", "user", 123);

        // Assert
        Assert.That(result, Is.EqualTo("entity:user:id=123"));
    }

    [Test]
    public void Format_WithNullParameter_ShouldUseNullString()
    {
        // Act
        var result = _formatter.Format("test:{0}", (object)null);

        // Assert
        Assert.That(result, Is.EqualTo("test:null"));
    }

    [Test]
    public void Format_WithBaseEntity_ShouldUseEntityId()
    {
        // Arrange
        var entity = new TestEntity { Id = 456 };

        // Act
        var result = _formatter.Format("entity:{0}", entity);

        // Assert
        Assert.That(result, Is.EqualTo("entity:456"));
    }

    [Test]
    public void Format_WithDecimal_ShouldUseInvariantCulture()
    {
        // Arrange - Test with German culture that uses comma separator
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");

            // Act
            var result = _formatter.Format("value:{0}", 3.14m);

            // Assert
            Assert.That(result, Is.EqualTo("value:3.14")); // Should use dot, not comma
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Test]
    public void Format_WithCollections_ShouldCreateOrderedHash()
    {
        // Arrange
        var ids = new[] { 3, 1, 5 };
        var entities = new[] { new TestEntity { Id = 30 }, new TestEntity { Id = 10 } };
        var parameters = new object[] {  entities };
        // Act
        var result1 = _formatter.Format("ids:{0}", ids);
        var result2 = _formatter.Format("entities:{0}", parameters);

        // Assert
        Assert.That(result1, Is.EqualTo("ids:1,3,5"));
        Assert.That(result2, Is.EqualTo("entities:10,30"));
    }

    [Test]
    public void Format_WithMixedParameters_ShouldHandleAll()
    {
        // Arrange
        var entity = new TestEntity { Id = 100 };

        // Act
        var result = _formatter.Format("{0}:{1}:{2}", "test", entity, new[] { 3, 1 });

        // Assert
        Assert.That(result, Is.EqualTo("test:100:1,3"));
    }

    [Test]
    public void Format_WithEmptyCollection_ShouldReturnEmptyString()
    {
        // Act & Assert
        var result = _formatter.Format("empty:{0}", Array.Empty<int>());
        Assert.That(result, Is.EqualTo("empty:"));
    }

    private class TestEntity : BaseEntity { }
}
