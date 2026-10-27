// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

using System.Runtime.CompilerServices;

namespace Soe.Build
{
    /// <summary>
    /// 
    /// </summary>
    #if EXPORT_HAMPER_CORE_SHARP
    public
    #else
    internal
    #endif
    static class ProcessingPipeline
    {
        private static ProcessingPhase current;
        /// <summary>
        /// Gets the current pipeline phase
        /// </summary>
        public static ProcessingPhase Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return current; }
        }
        
        /// <summary>
        /// Gets the current version of the provided processing stream
        /// </summary>
        /// <param name="phase">The <see cref="ProcessingPhase"/> indicating the stream</param>
        /// <returns>The current version of the provided processing stream</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCurrentVersion(ProcessingPhase phase)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the processing pipeline until all objects have been finished
        /// </summary>
        internal static async Task Update()
        {
            // Placeholder
            await Task.CompletedTask;
        }
    }
}