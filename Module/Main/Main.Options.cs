// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Soe.Runtime;

namespace Soe.Build
{
    // ReSharper disable ClassCannotBeInstantiated
    
    #if EXPORT_HAMPER_CORE_SHARP
    public
    #else
    internal
    #endif
    sealed partial class Main
    {
        [InlineArray(24)]
        private struct AvailableOptions
        {
            private const int DefaultCapacity = 24;
            private Option element0;
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Span<Option> AsSpan()
            {
                return MemoryMarshal.CreateSpan(ref element0, DefaultCapacity);
            }
        }

        /// <summary>
        /// Declares available command line options
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static AvailableOptions GetOptions( out int count)
        {
            AvailableOptions result = default;
            Span<Option> options = result.AsSpan();
            {
                options[0] = new Option("help", OptionFlags.Flag | OptionFlags.Optional, "Shows this help message");
                options[1] = new Option("h", OptionFlags.Alias, parameter: "help");
                options[2] = new Option("log", OptionFlags.RequiresParameter | OptionFlags.Optional, "Sets the log level to any of Silent:0 (default), Minimal:1 or Full:2");
                options[3] = new Option("l", OptionFlags.Alias, parameter: "log");
                options[4] = new Option("targetPath", OptionFlags.Verb | OptionFlags.Required | OptionFlags.NoHeader, "A collection of paths to include into the current build process");
            }
            count = 5;
            return result;
        }
    }
    
    // ReSharper restore ClassCannotBeInstantiated
}