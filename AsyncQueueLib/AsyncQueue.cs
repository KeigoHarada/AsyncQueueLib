using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncQueueLib
{
    /// <summary>
    /// 非同期キューの実装クラス
    /// </summary>
    /// <typeparam name="T">キューに格納する要素の型</typeparam>
    public class AsyncQueue<T> : IAsyncQueue<T>
    {
        private readonly ConcurrentQueue<T> _queue = new();
        private readonly ConcurrentQueue<TaskCompletionSource<T>> _waitingConsumers = new();
        private readonly object _lock = new();

        /// <summary>
        /// 非同期でアイテムをキューに追加します
        /// </summary>
        public Task EnqueueAsync(T item, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }

            lock (_lock)
            {
                // 待機中のConsumerがいれば、キャンセル済みをスキップしつつ直接ハンドオフ
                while (_waitingConsumers.TryDequeue(out var tcs))
                {
                    if (tcs.TrySetResult(item))
                    {
                        return Task.CompletedTask;
                    }
                    // キャンセル/完了済みは捨てて次へ
                }

                // 待機者がいなければキューに積む
                _queue.Enqueue(item);
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// 非同期でアイテムをキューから取得します
        /// </summary>
        public Task<T> DequeueAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<T>(cancellationToken);
            }

            lock (_lock)
            {
                // キューにアイテムがある場合は即座に返す
                if (_queue.TryDequeue(out var item))
                {
                    return Task.FromResult(item);
                }

                // キューが空の場合は待機
                var tcs = new TaskCompletionSource<T>();
                _waitingConsumers.Enqueue(tcs);

                // キャンセレーション登録（待機列は触らず、TCSのみ遷移）
                if (cancellationToken.CanBeCanceled)
                {
                    var registration = cancellationToken.Register(() =>
                    {
                        lock (_lock)
                        {
                            tcs.TrySetCanceled(cancellationToken);
                        }
                    });

                    // タスク完了時に登録を解除
                    tcs.Task.ContinueWith(
                        _ => registration.Dispose(),
                        TaskContinuationOptions.ExecuteSynchronously
                    );
                }

                return tcs.Task;
            }
        }

        /// <summary>
        /// 現在のキューサイズを取得します
        /// </summary>
        public int Count => _queue.Count;

        /// <summary>
        /// キューが空かどうかを取得します
        /// </summary>
        public bool IsEmpty => _queue.IsEmpty;
    }
}
