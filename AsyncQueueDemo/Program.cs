using System;
using System.Threading.Tasks;
using AsyncQueueLib;

namespace AsyncQueueDemo;

    /// <summary>
    /// AsyncQueueの使用例
    /// </summary>
    public static class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("=== AsyncQueue デモ ===");
            var queue = new AsyncQueue<int>();

            // Producer（生産者）タスク
            var producer = Task.Run(async () =>
            {
                for (int i = 1; i <= 5; i++)
                {
                    await queue.EnqueueAsync(i);
                    Console.WriteLine($"📤 生産: {i}");
                    await Task.Delay(500); // 生産の遅延
                }
            });

            // Consumer（消費者）タスク
            var consumer = Task.Run(async () =>
            {
                for (int i = 0; i < 5; i++)
                {
                    var item = await queue.DequeueAsync();
                    Console.WriteLine($"📥 消費: {item}");
                    await Task.Delay(300); // 消費の遅延
                }
            });

            // 両方のタスクが完了するまで待機
            await Task.WhenAll(producer, consumer);

            Console.WriteLine("✅ デモ完了！");
            Console.WriteLine($"最終的なキューサイズ: {queue.Count}");
        }
    }

