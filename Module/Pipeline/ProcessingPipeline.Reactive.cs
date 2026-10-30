// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

using System.Buffers;
using System.Runtime.CompilerServices;
using Soe.Collections.HashSet;
using Soe.Collections.Inline;
using Soe.Composable;
using Soe.Reactive;

namespace Soe.Build
{
    #if EXPORT_HAMPER_CORE_SHARP
    public
    #else
    internal
    #endif
    static partial class ProcessingPipeline
    {
        /// <summary>
        /// An async subscription strategy
        /// </summary>
        private struct SubscriptionStrategy : IAsyncSubscriptionStrategy<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>>
        {
            private SmallArray<IAsyncObserver<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>>?, SmallArray4<IAsyncObserver<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>>?>> subscribers;

            private int count;
            /// <inheritdoc/>
            public int Count
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return count; }
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Add(IAsyncObserver<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>> observer)
            {
                EnsureCapacity(++count);
                subscribers[count - 1] = observer;

                return true;
            }
            
            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose()
            {
                subscribers.Dispose();
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void EnsureCapacity(int capacity)
            {
                capacity = Math.Max(subscribers.Length, capacity);
                if (subscribers.Length < capacity)
                {
                    subscribers.Resize(capacity.NextPowerOfTwo());
                }
            }
            
            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Remove(IAsyncObserver<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>> observer)
            {
                int index = subscribers.IndexOf(observer);
                if (index >= 0 && index < subscribers.Length)
                {
                    subscribers[index] = subscribers[--count];
                    subscribers[count - 1] = null;
                }
                else if (index == subscribers.Length - 1)
                {
                    subscribers[--count] = null;
                }
            }
            
            #region ISubscriptionStrategy Members
            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            Task IAsyncSubscriptionStrategy<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>>.OnNextAsync<Dispatcher>(in Dispatcher dispatcher, in IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>> value)
            {
                return dispatcher.OnNextAsync(subscribers.AsSpan(), value);
            }
            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            Task IAsyncSubscriptionStrategy<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>>.OnErrorAsync<Dispatcher>(in Dispatcher dispatcher, in Exception error)
            {
                return dispatcher.OnErrorAsync(subscribers.AsSpan(), error);
            }
            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            Task IAsyncSubscriptionStrategy<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>>.OnCompletedAsync<Dispatcher>(in Dispatcher dispatcher)
            {
                return dispatcher.OnCompletedAsync(subscribers.AsSpan());
            }
            #endregion
        }
        
        /// <summary>
        /// An async dispatching strategy
        /// </summary>
        private struct DispatchingStrategy : IAsyncDispatchingStrategy<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>>
        {
            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose()
            { }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            async Task Dispatch(Task[] tasks, int count)
            {
                try
                {
                    await Task.WhenAll(tasks.Take(count));
                }
                finally
                {
                    ArrayPool<Task>.Shared.Return(tasks);
                }
            }
            
            #region IDispatchingStrategy Members
            /// <inheritdoc/>
            Task IAsyncDispatchingStrategy<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>>.OnNextAsync(Span<IAsyncObserver<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>>?> observers, in IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>> value)
            {
                Task[] tasks = ArrayPool<Task>.Shared.Rent(observers.Length);
                int count = 0;
                
                // Build task list
                foreach (var observer in observers)
                {
                    if (observer != null)
                    {
                        tasks[count++] = observer!.OnNextAsync(value);
                    }
                }
                return Dispatch(tasks, count);
            }
            /// <inheritdoc/>
            Task IAsyncDispatchingStrategy<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>>.OnErrorAsync(Span<IAsyncObserver<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>>?> observers, in Exception error)
            {
                Task[] tasks = ArrayPool<Task>.Shared.Rent(observers.Length);
                int count = 0;
                
                // Build task list
                foreach (var observer in observers)
                {
                    if (observer != null)
                    {
                        tasks[count++] = observer!.OnErrorAsync(error);
                    }
                }
                return Dispatch(tasks, count);
            }
            /// <inheritdoc/>
            Task IAsyncDispatchingStrategy<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>>.OnCompletedAsync(Span<IAsyncObserver<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>>?> observers)
            {
                Task[] tasks = ArrayPool<Task>.Shared.Rent(observers.Length);
                int count = 0;
                
                // Build task list
                foreach (var observer in observers)
                {
                    if (observer != null)
                    {
                        tasks[count++] = observer!.OnCompletedAsync();
                    }
                }
                return Dispatch(tasks, count);
            }
            #endregion
        }
    }
}