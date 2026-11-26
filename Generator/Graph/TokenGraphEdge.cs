// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

using System.Runtime.CompilerServices;
using Soe.Unicode;

namespace Soe.Build.Generator
{
    /// <summary>
    /// A strictly directed acyclic graph edge between two nodes
    /// </summary>
    #if EXPORT_HAMPER_BUILD_GENERATOR
    public
    #else
    internal
    #endif
    readonly struct TokenGraphEdge : IEquatable<TokenGraphEdge>
    {
        private readonly int id;
        /// <summary>
        /// The unique node ID
        /// </summary>
        public int Id
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return id; }
        }
        
        private readonly Char32 incoming;
        /// <summary>
        /// The previous node this edge has come from
        /// </summary>
        public Char32 Incoming
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return incoming; }
        }

        private readonly Char32 outgoing;
        /// <summary>
        /// The succeeding node this edge is forwarded to
        /// </summary>
        public Char32 Outgoing
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return outgoing; }
        }

        private readonly int slice;
        /// <summary>
        /// The slice ID
        /// </summary>
        public int Slice
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return slice; }
        }

        /// <summary>
        /// Initializes a new directed edge from one node to another 
        /// </summary>
        /// <param name="id">The unique edge ID</param>
        /// <param name="slice">The slice ID this edge belongs to</param>
        /// <param name="incoming">The previous node this edge has come from</param>
        /// <param name="outgoing">The succeeding node this edge is forwarded to</param>
        public TokenGraphEdge(int id, int slice, Char32 incoming, Char32 outgoing)
        {
            this.id = id;
            this.slice = slice;
            this.incoming = incoming;
            this.outgoing = outgoing;
        }
        /// <summary>
        /// Initializes a new directed edge from one the root state to another node 
        /// </summary>
        /// <param name="id">The unique edge ID</param>
        /// <param name="slice">The slice ID this edge belongs to</param>
        /// <param name="outgoing">The succeeding node this edge is forwarded to</param>
        public TokenGraphEdge(int id, int slice, Char32 outgoing)
            :this(id, slice, default, outgoing)
        { }
        
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(TokenGraphEdge other)
        {
            return (id == other.id && incoming.Equals(other.incoming) && outgoing.Equals(other.outgoing));
        }
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj)
        {
            return (obj is TokenGraphEdge other && Equals(other));
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return HashCode.Combine(id, incoming, outgoing);
        }
        
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return string.Concat(incoming == 0 ? string.Empty : incoming, " =(", outgoing, ")=> ", slice);
        }
    }
}
