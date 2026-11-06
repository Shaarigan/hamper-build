// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

using System.Runtime.CompilerServices;
using Soe.Runtime;
using Soe.Storage;

namespace Soe.Build
{
    /// <summary>
    /// Provides the target root of a workspace
    /// </summary>
    /// <param name="source">The source string to treat as workspace root</param>
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    #if EXPORT_HAMPER_BUILD_MAIN
    public
    #else
    internal
    #endif
    readonly struct WorkspaceComponent(string source, WorkspaceType type)
    {
        private readonly WorkspaceType type = type;
        /// <summary>
        /// Gets the type of this workspace
        /// </summary>
        public WorkspaceType Type
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return type; }
        }
        
        private readonly ResourceIdentifier root = new ResourceIdentifier(source, ResourceIdentifier.Parse(source));
        /// <summary>
        /// Gets the unified resource identifier of the workspace root
        /// </summary>
        public ResourceIdentifier Root
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return root; }
        }
    }
}