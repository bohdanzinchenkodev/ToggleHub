using ToggleHub.Infrastructure.Cache;

namespace ToggleHub.Infrastructure.UnitTests.Services
{
    [TestFixture]
    public class CacheKeyRegistryTests
    {
        private InMemoryCacheKeyRegistry _registry = null!;

        [SetUp]
        public void Setup()
        {
            _registry = new InMemoryCacheKeyRegistry();
        }

        [Test]
        public void AddKey_ShouldStoreKey()
        {
            _registry.AddKey("entity:project:1");

            Assert.That(_registry.GetKeys(), Contains.Item("entity:project:1"));
        }

        [Test]
        public void RemoveKey_ShouldRemoveKey()
        {
            _registry.AddKey("entity:project:1");
            _registry.RemoveKey("entity:project:1");

            Assert.That(_registry.GetKeys(), Does.Not.Contain("entity:project:1"));
        }

        [Test]
        public void AddKey_Twice_ShouldStillContainOnce()
        {
            _registry.AddKey("entity:project:1");
            _registry.AddKey("entity:project:1");

            var keys = _registry.GetKeys();
            Assert.That(keys.Count(k => k == "entity:project:1"), Is.EqualTo(1));
        }

        [Test]
        public void GetAllKeys_ShouldReturnAllKeys()
        {
            _registry.AddKey("a");
            _registry.AddKey("b");

            var keys = _registry.GetKeys();

            Assert.That(keys, Does.Contain("a"));
            Assert.That(keys, Does.Contain("b"));
        }
    }
}