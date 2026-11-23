// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

using System.Runtime.CompilerServices;
using Soe.Storage;

namespace Soe.Build
{
    /// <summary>
    /// Provides the root of a C# project
    /// </summary>
    /// <param name="identifier">The source to treat as the root</param>
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    #if EXPORT_HAMPER_BUILD_SHARP
    public
    #else
    internal
    #endif
    readonly struct SharpProjectComponent(ResourceIdentifier identifier)
    {
        /// <summary>
        /// Gets the unified resource identifier for this project's root
        /// </summary>
        public ResourceIdentifier Identifier
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return identifier; }
        }
    }
}