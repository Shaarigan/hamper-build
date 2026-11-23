// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

using System.Runtime.CompilerServices;
using Soe.Composable;
using Soe.Runtime;

[assembly: Submodule<Soe.Build.SharpPlugin>("Build")]
namespace Soe.Build
{
    // ReSharper disable ClassCannotBeInstantiated
    // ReSharper disable ClassNeverInstantiated.Global
    
    /// <summary>
    /// Declares the main instance for this plugin
    /// </summary>
    #if EXPORT_HAMPER_BUILD_SHARP
    public
    #else
    internal
    #endif
    sealed class SharpPlugin : IBuildService
    {
        private static UInt32 signal;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SharpPlugin()
        { }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Load()
        {
            signal = 0;
        }
        
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Activate()
        {
            if(Interlocked.CompareExchange(ref signal, 1, 0) == 0)
            {
                if (Shard.TryGetShard(Main.StorageId, out Shard? shard))
                {
                    // Register additional components
                    shard!.RegisterComponent<SharpProjectComponent>();

                    // Register additional services
                    ProcessingPipeline.DiscoveryStream.Subscribe(Sharp.ProjectService.Instance);
                }
                else Application.Log(VerbosityFlags.Error, SeverityFlags.Silent, $"Failed to initialize {nameof(SharpPlugin)}");
            }
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Unload()
        { }
    }
    
    // ReSharper restore ClassNeverInstantiated.Global
    // ReSharper restore ClassCannotBeInstantiated
}