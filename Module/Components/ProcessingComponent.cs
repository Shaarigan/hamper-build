// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

using System.Runtime.CompilerServices;

namespace Soe.Build
{
    /// <summary>
    /// Provides properties read by the <see cref="ProcessingPipeline"/>
    /// </summary>
    /// <param name="phase">The processing phase requested by the object</param>
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    #if EXPORT_HAMPER_CORE_SHARP
    public
    #else
    internal
    #endif
    readonly struct ProcessingComponent(ProcessingPhase phase)
    {
        /// <summary>
        /// Gets the processing pahse currently requested
        /// </summary>
        public readonly ProcessingPhase Phase
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return phase; }
        }

        private readonly int version = ProcessingPipeline.GetCurrentVersion(phase);
        /// <summary>
        /// Gets the version of the requested processing phase
        /// </summary>
        public int Version
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return version; }
        }
    }
}