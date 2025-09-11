# AsyncQueueLib

軽量な非同期Producer/Consumer用キュー。

## インストール

```bash
dotnet add package AsyncQueueLib
```

## 最小サンプル

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using AsyncQueueLib;

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

var consumer = Task.Run(async () =>
{
    try
    {
        while (true)
        {
            var msg = await queue.DequeueAsync(cts.Token);
            Console.WriteLine(msg);
        }
    }
    catch (OperationCanceledException) { }
});

await consumer;
```

## 特長

- 待機者への直接ハンドオフで低レイテンシ
- 安全なキャンセル（キャンセル済み待機者は自然にスキップ）
- スレッドセーフ（`lock`で整合性を担保）

## ライセンス

MIT
