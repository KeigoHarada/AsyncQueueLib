using System;
using System.Threading;
using System.Threading.Tasks;
using AsyncQueueLib;

internal class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== 最小デモ: Producer/Consumer ===");

        var queue = new AsyncQueue<string>();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

        _ = Task.Run(async () =>
        {
            var i = 0;
            while (!cts.IsCancellationRequested)
            {
                await queue.EnqueueAsync($"msg-{++i}");
                await Task.Delay(50);
            }
        });

        var c1 = Task.Run(async () =>
        {
            try
            {
                while (true)
                {
                    var msg = await queue.DequeueAsync(cts.Token);
                    Console.WriteLine($"C1: {msg}");
                }
            }
            catch (OperationCanceledException) { }
        });

        var c2 = Task.Run(async () =>
        {
            try
            {
                while (true)
                {
                    var msg = await queue.DequeueAsync(cts.Token);
                    Console.WriteLine($"C2: {msg}");
                }
            }
            catch (OperationCanceledException) { }
        });

        await Task.WhenAll(c1, c2);
        Console.WriteLine("✅ 完了");
    }
}
