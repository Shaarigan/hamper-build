// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

using System.Runtime.CompilerServices;
using Soe.Collections.HashSet;
using Soe.Composable;

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
        private class PipelineAdapter : IReadOnlyIterable<EntityId, ReadOnlyIterator<EntityId>.DefaultStrategy>, IDisposable
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
            /// 
            /// </summary>
            /// <param name="entity"></param>
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
            }

            public async Task Flush()
            {
                version++;
                {
                    // Placeholder
                    await Task.CompletedTask;
                }
                entities.Clear();
                count = 0;
            }
            
            #region IReadOnlyIterable Members
            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ReadOnlyIterator<EntityId, ReadOnlyIterator<EntityId>.DefaultStrategy> IReadOnlyIterable<EntityId, ReadOnlyIterator<EntityId>.DefaultStrategy>.GetEnumerator()
            {
                throw new NotImplementedException();
            }
            #endregion
        }

        private static readonly PipelineAdapter discoveryAdapter;
        /// <summary>
        /// A reactive data stream populating <see cref="ProcessingPhase.Discovery"/> updates
        /// </summary>
        public static IObservable<IReadOnlyIterable<EntityId, ReadOnlyIterator<EntityId>.DefaultStrategy>> DiscoveryStream
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { throw new NotImplementedException(); }
        }
        
        private static readonly PipelineAdapter parsingAdapter;
        /// <summary>
        /// A reactive data stream populating <see cref="ProcessingPhase.Parsing"/> updates
        /// </summary>
        public static IObservable<IReadOnlyIterable<EntityId, ReadOnlyIterator<EntityId>.DefaultStrategy>> ParsingStream
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { throw new NotImplementedException(); }
        }
        
        private static readonly PipelineAdapter lintingAdapter;
        /// <summary>
        /// A reactive data stream populating <see cref="ProcessingPhase.Linting"/> updates
        /// </summary>
        public static IObservable<IReadOnlyIterable<EntityId, ReadOnlyIterator<EntityId>.DefaultStrategy>> LintingStream
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { throw new NotImplementedException(); }
        }
        
        private static readonly PipelineAdapter analysisAdapter;
        /// <summary>
        /// A reactive data stream populating <see cref="ProcessingPhase.Analysis"/> updates
        /// </summary>
        public static IObservable<IReadOnlyIterable<EntityId, ReadOnlyIterator<EntityId>.DefaultStrategy>> AnalysisStream
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { throw new NotImplementedException(); }
        }
        
        private static readonly PipelineAdapter classificationAdapter;
        /// <summary>
        /// A reactive data stream populating <see cref="ProcessingPhase.Classification"/> updates
        /// </summary>
        public static IObservable<IReadOnlyIterable<EntityId, ReadOnlyIterator<EntityId>.DefaultStrategy>> ClassificationStream
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { throw new NotImplementedException(); }
        }
        
        private static readonly PipelineAdapter buildAdapter;
        /// <summary>
        /// A reactive data stream populating <see cref="ProcessingPhase.BuildOrchestration"/> updates
        /// </summary>
        public static IObservable<IReadOnlyIterable<EntityId, ReadOnlyIterator<EntityId>.DefaultStrategy>> BuildStream
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { throw new NotImplementedException(); }
        }
        
        private static readonly PipelineAdapter exportAdapter;
        /// <summary>
        /// A reactive data stream populating <see cref="ProcessingPhase.Export"/> updates
        /// </summary>
        public static IObservable<IReadOnlyIterable<EntityId, ReadOnlyIterator<EntityId>.DefaultStrategy>> ExportStream
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { throw new NotImplementedException(); }
        }
    }
}