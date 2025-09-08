using System.Threading.Tasks;
using AsyncQueueLib;
using Xunit;

namespace AsyncQueueLib.Tests
{
    public class AsyncQueueTests
    {
        [Fact]
        public async Task EnqueueAsync_ShouldAddItem()
        {
            // Arrange
            var queue = new AsyncQueue<int>();

            // Act
            await queue.EnqueueAsync(42);

            // Assert
            Assert.Equal(1, queue.Count);
            Assert.False(queue.IsEmpty);
        }

        [Fact]
        public async Task DequeueAsync_ShouldReturnItem()
        {
            // Arrange
            var queue = new AsyncQueue<int>();
            await queue.EnqueueAsync(42);

            // Act
            var result = await queue.DequeueAsync();

            // Assert
            Assert.Equal(42, result);
            Assert.Equal(0, queue.Count);
            Assert.True(queue.IsEmpty);
        }

        [Fact]
        public async Task DequeueAsync_ShouldWaitForItem()
        {
            // Arrange
            var queue = new AsyncQueue<int>();
            var dequeueTask = queue.DequeueAsync();

            // Act
            await Task.Delay(100); // 少し待機
            await queue.EnqueueAsync(42);
            var result = await dequeueTask;

            // Assert
            Assert.Equal(42, result);
        }

        [Fact]
        public async Task MultipleEnqueueDequeue_ShouldWork()
        {
            // Arrange
            var queue = new AsyncQueue<int>();

            // Act
            await queue.EnqueueAsync(1);
            await queue.EnqueueAsync(2);
            await queue.EnqueueAsync(3);

            // Assert
            Assert.Equal(3, queue.Count);

            var item1 = await queue.DequeueAsync();
            var item2 = await queue.DequeueAsync();
            var item3 = await queue.DequeueAsync();

            Assert.Equal(1, item1);
            Assert.Equal(2, item2);
            Assert.Equal(3, item3);
            Assert.True(queue.IsEmpty);
        }
    }
}
