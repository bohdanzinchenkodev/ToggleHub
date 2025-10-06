using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ToggleHub.Application.EventHandlers;
using ToggleHub.Domain.Events;
using ToggleHub.Infrastructure.Services;

namespace ToggleHub.Infrastructure.UnitTests.Services
{
    public class TestEvent : BaseEvent { }

    public interface ITestConsumer : IConsumer<TestEvent> { }

    [TestFixture]
    public class EventPublisherTests
    {
        [Test]
        public async Task PublishAsync_ShouldCallAllConsumers()
        {
            // arrange
            var evt = new TestEvent();

            var consumer1 = new Mock<IConsumer<TestEvent>>();
            consumer1.Setup(c => c.HandleEventAsync(evt)).Returns(Task.CompletedTask).Verifiable();

            var consumer2 = new Mock<IConsumer<TestEvent>>();
            consumer2.Setup(c => c.HandleEventAsync(evt)).Returns(Task.CompletedTask).Verifiable();

            var services = new ServiceCollection();
            services.AddSingleton(consumer1.Object);
            services.AddSingleton(consumer2.Object);

            var sp = services.BuildServiceProvider();
            var logger = Mock.Of<ILogger<EventPublisher>>();

            var publisher = new EventPublisher(sp, logger);

            // act
            await publisher.PublishAsync(evt);

            // assert
            consumer1.Verify(c => c.HandleEventAsync(evt), Times.Once);
            consumer2.Verify(c => c.HandleEventAsync(evt), Times.Once);
        }

        [Test]
        public async Task PublishAsync_ShouldCatchExceptionAndLog()
        {
            var evt = new TestEvent();

            var failingConsumer = new Mock<IConsumer<TestEvent>>();
            failingConsumer
                .Setup(c => c.HandleEventAsync(evt))
                .ThrowsAsync(new InvalidOperationException("fail"));

            var logger = new Mock<ILogger<EventPublisher>>();

            var services = new ServiceCollection();
            services.AddSingleton(failingConsumer.Object);

            var sp = services.BuildServiceProvider();

            var publisher = new EventPublisher(sp, logger.Object);

            // act
            await publisher.PublishAsync(evt);

            // assert: no exception thrown, error logged
            logger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Error occurred while handling event")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
