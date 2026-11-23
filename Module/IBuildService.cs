// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

using Soe.Runtime;

namespace Soe.Build
{
    /// <summary>
    /// Declares a service module providing data and operations consumed by other modules
    /// </summary>
    #if EXPORT_HAMPER_BUILD_SHARP
    public
    #else
    internal
    #endif
    interface IBuildService : IRuntimeModule
    {
        /// <summary>
        /// A callback executed by dependency modules
        /// </summary>
        static abstract void Activate();
    }
}