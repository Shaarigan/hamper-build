// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

using System.Runtime.CompilerServices;
using Soe.Collections.Inline;
using Soe.Reactive;
using Soe.Runtime;

namespace Soe.Build
{
    // ReSharper disable ClassCannotBeInstantiated
    
    #if EXPORT_HAMPER_CORE_SHARP
    public
    #else
    internal
    #endif
    sealed partial class Main
    {
        /// <summary>
        /// The default dispatching strategy for this source
        /// </summary>
        public struct DispatchingStrategy : IDispatchingStrategy<CommandFragment>
        {
            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose()
            { }
            
            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnNext(Span<IObserver<CommandFragment>?> observers, in CommandFragment value)
            {
                foreach (IObserver<CommandFragment>? observer in observers)
                {
                    observer?.OnNext(value);
                }
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnError(Span<IObserver<CommandFragment>?> observers, in Exception error)
            {
                foreach (IObserver<CommandFragment>? observer in observers)
                {
                    observer?.OnError(error);
                }
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted(Span<IObserver<CommandFragment>?> observers)
            {
                foreach (IObserver<CommandFragment>? observer in observers)
                {
                    observer?.OnCompleted();
                }
            }
        }

        /// <summary>
        /// The default subscription strategy for this source
        /// </summary>
        public struct SubscriptionStrategy : ISubscriptionStrategy<CommandFragment>
        {
            SmallArray<IObserver<CommandFragment>?, SmallArray4<IObserver<CommandFragment>?>> subscribers;

            private int count;
            /// <inheritdoc/>
            public int Count
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return count; }
            }
            
            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Add(IObserver<CommandFragment> observer)
            {
                EnsureCapacity(count);
                subscribers[count++] = observer;
                
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
            public void OnNext<Dispatcher>(in Dispatcher dispatcher, in CommandFragment value) 
                where Dispatcher : IDispatchingStrategy<CommandFragment>
            {
                dispatcher.OnNext(subscribers.AsSpan(), in value);
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnError<Dispatcher>(in Dispatcher dispatcher, in Exception error) 
                where Dispatcher : IDispatchingStrategy<CommandFragment>
            {
                dispatcher.OnError(subscribers.AsSpan(), in error);
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted<Dispatcher>(in Dispatcher dispatcher) 
                where Dispatcher : IDispatchingStrategy<CommandFragment>
            {
                dispatcher.OnCompleted(subscribers.AsSpan());
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Remove(IObserver<CommandFragment> observer)
            {
                int index = subscribers.IndexOf(observer);
                if (index >= 0)
                {
                    subscribers[index] = subscribers[count - 1];
                    subscribers[--count] = null;
                }
            }
        }
    }
    
    // ReSharper restore ClassCannotBeInstantiated
}