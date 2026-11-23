// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Soe.Collections.Embedded;
using Soe.Collections.HashSet;
using Soe.Composable;
using Soe.Reactive;
using Soe.Runtime;
using Soe.Storage;
using Soe.Threading;

namespace Soe.Build.Sharp
{
    // ReSharper disable ClassCannotBeInstantiated
    // ReSharper disable ClassNeverInstantiated.Global
    
    /// <summary>
    /// Provides C# project discovery
    /// </summary>
    #if EXPORT_HAMPER_BUILD_SHARP
    public
    #else
    internal
    #endif
    sealed class ProjectService : IAsyncObserver<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>>
    {
        public static readonly string[] Filter =
        [
            "*[/\\]parcel.json",
            "*.cs"
        ];
        
        private static readonly ProjectService instance;
        /// <summary>
        /// Gets this service's default instance
        /// </summary>
        public static ProjectService Instance
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return instance; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ProjectService()
        {
            instance = new ProjectService();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ProjectService()
        { }

        static bool DiscoverProject(ref EmbeddedList<string, PoolArray<string>> postProcess, in EntityId entity, PathDescriptor path)
        {
            if (path.Exists)
            {
                using (StorageIterator iterator = path.GetFiles("*.*", SearchOption.AllDirectories))
                using (EmbeddedList<string, PoolArray<string>> projects = default)
                {
                    while (iterator.MoveNext())
                    {
                        ReadOnlySpan<char> parent = iterator.Current.Slice(path.Length);
                        if(parent.IsMatch(Filter))
                        {
                            parent = ResourceIdentifier.GetParent(parent);
                            
                            if(parent.Length > 1)
                            {
                                for (int i = 0; i < projects.Count; i++)
                                {
                                    if (projects[i].Length >= parent.Length)
                                    {
                                        if (projects[i].AsSpan().StartsWith(parent))
                                        {
                                            projects.RemoveAt(i);
                                        }
                                    }
                                    else if (parent.StartsWith(projects[i]))
                                    {
                                        goto Next;
                                    }
                                }
                                projects.Add(parent.ToString());

                            Next: ;
                            }
                            else
                            {
                                postProcess.Add(path);
                                return true;
                            }
                        }
                    }
                    
                    // Prepare for entity creation in post-processing step
                    for (int i = 0; i < projects.Count; i++)
                    {
                        postProcess.Add(ResourceIdentifier.Normalize(((ResourceIdentifier)path) / projects[i], NormalizationFlags.Default));
                    }
                    return (projects.Count > 0);
                }
            }
            return false;
        }
        
        static bool ResolveTarget(ref EmbeddedList<string, PoolArray<string>> postProcess, in EntityId entity, in TargetComponent targetComponent)
        {
            ResourceIdentifier target = targetComponent.Identifier;
            if(target.IsLocation && !target.Location.IsUnc)
            {
                ResourceIdentifier.LocationComponent location = target.Location;
                if (location.Authority.IsEmpty || (location.IsUri && location.Scheme.SequenceEqual("file", ResourceIdentifier.OrdnialComparer.Default)))
                {
                    return DiscoverProject(ref postProcess, in entity, StorageProvider.Default.GetDirectory(target.Target.FullName));
                }
                else if (!location.Authority.IsEmpty && VolumeManager.TryGetVolume(location.Authority.ToString(), out ResourceIdentifier volume) && !volume.IsLocation)
                {
                    return DiscoverProject(ref postProcess, in entity, StorageProvider.Default.GetDirectory(volume / target));
                }
                else return false;
            }
            else if (!target.IsLocation)
            {
                return DiscoverProject(ref postProcess, in entity, StorageProvider.Default.GetDirectory(target.Target.FullName));
            }
            else return false;
        }
        
        #region AsyncObserver Members
        /// <inheritdoc/>
        async Task IAsyncObserver<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>>.OnNextAsync(IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>> value)
        {
            if (Shard.TryGetShard(Main.StorageId, out Shard? shard))
            {
                Component<TargetComponent> targetComponent = shard!.GetComponent<TargetComponent>()!;
                Component<SharpProjectComponent> projectComponent = shard!.GetComponent<SharpProjectComponent>()!;
                Component<ProcessingComponent> processingComponent = shard!.GetComponent<ProcessingComponent>()!;
                
                EmbeddedList<string, PoolArray<string>> postProcess = default;
                try
                {
                    // Resolve targets
                    using (await (targetComponent).BorrowAsync(default(ImmutablePolicy)))
                    {
                        foreach (EntityId entity in value)
                        {
                            if (entity.TryGetComponent(out Ref<TargetComponent> target) && ResolveTarget(ref postProcess, in entity, in target.Value))
                            {
                                // Signals the target to be processed by this service
                                target.Value.Resolve();
                            }
                        }
                    }

                    // Create entities in a second pass to not unnecessarily block other services
                    using (await (shard.Entities, processingComponent, projectComponent).BorrowAsync(default(MutablePolicy), default(MutablePolicy), default(MutablePolicy)))
                    {
                        foreach (string path in postProcess)
                        {
                            EntityId entity = shard.Entities.Create();
                            entity.AddComponent<SharpProjectComponent>() = new SharpProjectComponent(path);
                            entity.AddComponent<ProcessingComponent>() = new ProcessingComponent(ProcessingPhase.Parsing);
                            
                            Application.Log(VerbosityFlags.Message, SeverityFlags.Full, string.Concat("CSharp: Located '", path, "'"));
                        }
                    }
                }
                finally
                {
                    postProcess.Dispose();
                }
            }
        }
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Task IAsyncObserver<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>>.OnErrorAsync(Exception error)
        {
            return Task.CompletedTask;
        }
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Task IAsyncObserver<IReadOnlyIterable<HashSet.DefaultContainer<EntityId>, HashSet.ReadOnlyIteratorStrategy<EntityId, HashSet.DefaultContainer<EntityId>>>>.OnCompletedAsync()
        {
            return Task.CompletedTask;
        }
        #endregion
    }
    
    // ReSharper restore ClassNeverInstantiated.Global
    // ReSharper restore ClassCannotBeInstantiated
}