using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncQueueLib
{
    /// <summary>
    /// 非同期キューインターフェース
    /// </summary>
    /// <typeparam name="T">キューに格納する要素の型</typeparam>
    public interface IAsyncQueue<T>
    {
        /// <summary>
        /// 非同期でアイテムをキューに追加します
        /// </summary>
        /// <param name="item">追加するアイテム</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        /// <returns>非同期操作を表すタスク</returns>
        Task EnqueueAsync(T item, CancellationToken cancellationToken = default);

        /// <summary>
        /// 非同期でアイテムをキューから取得します
        /// キューが空の場合は、アイテムが追加されるまで待機します
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークン</param>
        /// <returns>取得したアイテム</returns>
        Task<T> DequeueAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 現在のキューサイズを取得します
        /// </summary>
        int Count { get; }

        /// <summary>
        /// キューが空かどうかを取得します
        /// </summary>
        bool IsEmpty { get; }
    }
}
