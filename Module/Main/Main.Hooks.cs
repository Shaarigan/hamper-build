// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

using System.Runtime.CompilerServices;
using Soe.Collections.Embedded;
using Soe.Reactive;
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
        private static Anchor<CommandFragment, Anchor<CommandFragment>.DefaultStrategy<HeapArray<EmbeddedSortedList<INotificationHook<CommandFragment>>.ListNode>>> commandLineAnchor;

        /// <summary>
        /// Interception point handling command line arguments
        /// </summary>
        public static AnchorHandle<CommandFragment, Anchor<CommandFragment, Anchor<CommandFragment>.DefaultStrategy<HeapArray<EmbeddedSortedList<INotificationHook<CommandFragment>>.ListNode>>>> CommandLine
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new AnchorHandle<CommandFragment, Anchor<CommandFragment, Anchor<CommandFragment>.DefaultStrategy<HeapArray<EmbeddedSortedList<INotificationHook<CommandFragment>>.ListNode>>>>(ref commandLineAnchor); }
        }
    }
    
    // ReSharper restore ClassCannotBeInstantiated
}