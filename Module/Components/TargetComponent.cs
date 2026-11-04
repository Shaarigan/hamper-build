// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

using System.Runtime.CompilerServices;
using Soe.Storage;

namespace Soe.Build
{
    /// <summary>
    /// Provides a target for source identification in <see cref="ProcessingPhase.Discovery"/>
    /// </summary>
    /// <param name="source">The source string to treat as a path</param>
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    #if EXPORT_HAMPER_BUILD_MAIN
    public
    #else
    internal
    #endif
    readonly struct TargetComponent(string source)
    {
        private readonly ResourceIdentifier identifier = new ResourceIdentifier(source, ResourceIdentifier.Parse(source));
        /// <summary>
        /// Gets the unified resource identifier for this target
        /// </summary>
        public ResourceIdentifier Identifier
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return identifier; }
        }
    }
}