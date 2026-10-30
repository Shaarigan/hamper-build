// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

using System.Runtime.CompilerServices;
using Soe.Collections.HashSet;
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
        /// An adapter handling subscriptions and processing data
        /// </summary>
        private class PipelineAdapter : AsyncSource<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>, DispatchingStrategy, SubscriptionStrategy>, IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>, IDisposable
        {
            private PoolArray<HashSet.DefaultContainer<EntityId>> entities;
            private int count;
            /// <summary>
            /// Gets the current number of entities awaiting processing
            /// </summary>
            public int Count
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return count; }
            }
            
            private int version;
            /// <summary>
            /// Gets the current container version
            /// </summary>
            /// <remarks>Version is increased whenever the container layout changes</remarks>
            public int Version
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return version; }
            }
            
            /// <summary>
            /// Adds an object to the list of objects ready for processing
            /// </summary>
            /// <param name="entity">An object ready for processing</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(in EntityId entity)
            {
                if (!HashSet.Find<EntityId, PoolArray<HashSet.DefaultContainer<EntityId>>, HashSet.DefaultContainer<EntityId>>(ref entities, count, EqualityComparer<EntityId>.Default, in entity, entity.GetHashCode(), out int index, out int distance, out _))
                {
                    int tmp = 0;
                    HashSet.Emplace<EntityId, PoolArray<HashSet.DefaultContainer<EntityId>>, HashSet.DefaultContainer<EntityId>>(ref entities, ref count, EqualityComparer<EntityId>.Default, in entity, entity.GetHashCode(), index, distance, tmp, ref tmp);
                }
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose()
            {
                entities.Dispose();
                version = 0;
                count = 0;
            }

            /// <summary>
            /// Flushes the reactive data stream and starts processing
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public async Task Flush()
            {
                version++;
                {
                    await OnNextAsync(this);
                }
                entities.Clear();
                count = 0;
            }
            
            #region IReadOnlyIterable Members
            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ReadOnlyIterator<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>> IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>.GetEnumerator()
            {
                return new ReadOnlyIterator<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>(entities.AsSpan());
            }
            #endregion
        }

        private static readonly PipelineAdapter discoveryAdapter;
        /// <summary>
        /// A reactive data stream populating <see cref="ProcessingPhase.Discovery"/> updates
        /// </summary>
        public static IAsyncObservable<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>> DiscoveryStream
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return discoveryAdapter; }
        }
        
        private static readonly PipelineAdapter parsingAdapter;
        /// <summary>
        /// A reactive data stream populating <see cref="ProcessingPhase.Parsing"/> updates
        /// </summary>
        public static IAsyncObservable<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>> ParsingStream
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return parsingAdapter; }
        }
        
        private static readonly PipelineAdapter lintingAdapter;
        /// <summary>
        /// A reactive data stream populating <see cref="ProcessingPhase.Linting"/> updates
        /// </summary>
        public static IAsyncObservable<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>> LintingStream
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return lintingAdapter; }
        }
        
        private static readonly PipelineAdapter analysisAdapter;
        /// <summary>
        /// A reactive data stream populating <see cref="ProcessingPhase.Analysis"/> updates
        /// </summary>
        public static IAsyncObservable<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>> AnalysisStream
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return analysisAdapter; }
        }
        
        private static readonly PipelineAdapter classificationAdapter;
        /// <summary>
        /// A reactive data stream populating <see cref="ProcessingPhase.Classification"/> updates
        /// </summary>
        public static IAsyncObservable<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>> ClassificationStream
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return classificationAdapter; }
        }
        
        private static readonly PipelineAdapter buildAdapter;
        /// <summary>
        /// A reactive data stream populating <see cref="ProcessingPhase.BuildOrchestration"/> updates
        /// </summary>
        public static IAsyncObservable<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>> BuildStream
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return buildAdapter; }
        }
        
        private static readonly PipelineAdapter exportAdapter;
        /// <summary>
        /// A reactive data stream populating <see cref="ProcessingPhase.Export"/> updates
        /// </summary>
        public static IAsyncObservable<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>> ExportStream
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return exportAdapter; }
        }
    }
}