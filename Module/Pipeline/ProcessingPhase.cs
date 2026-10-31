// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

namespace Soe.Build
{
    /// <summary>
    /// Defines the available pipeline states
    /// </summary>
    #if EXPORT_HAMPER_BUILD_MAIN
    public
    #else
    internal
    #endif
    enum ProcessingPhase
    {
        /// <summary>
        /// Processes sources into fragment data
        /// </summary>
        Discovery,
        /// <summary>
        /// Processes fragment data into token streams
        /// </summary>
        Parsing,
        /// <summary>
        /// Verifies token streams
        /// </summary>
        Linting, 
        /// <summary>
        /// Processes fragment data
        /// </summary>
        Analysis, 
        /// <summary>
        /// Assigns fragment data to the corresponding process
        /// </summary>
        Classification,
        /// <summary>
        /// Performs the processing of fragments
        /// </summary>
        BuildOrchestration,
        /// <summary>
        /// Arranges artifacts resulting from a previous process
        /// </summary>
        Export
    }
}