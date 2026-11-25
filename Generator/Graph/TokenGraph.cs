// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Soe.Collections.Embedded;
using Soe.Collections.HashSet;
using Soe.Unicode;

namespace Soe.Build.Generator
{
    /// <summary>
    /// A strictly directed acyclic graph of a finite token hierarchy
    /// </summary>
    #if EXPORT_HAMPER_BUILD_GENERATOR
    public
    #else
    internal
    #endif
    struct TokenGraph<ArrayBuffer>() : IDisposable
        where ArrayBuffer : struct, IArrayAccessor<HashSet.DefaultContainer<TokenGraphEdge>>
    {
        private HashSet<TokenGraphEdge, ArrayBuffer, HashSet.DefaultContainer<TokenGraphEdge>> edges = new HashSet<TokenGraphEdge, ArrayBuffer, HashSet.DefaultContainer<TokenGraphEdge>>(EqualityComparer<TokenGraphEdge>.Default);

        /// <summary>
        /// Gets the current number of edges in this graph
        /// </summary>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return edges.Count; }
        }

        /// <summary>
        /// Extends this graph by the collection of items
        /// </summary>
        /// <param name="items">A collection of items to add to this graph</param>
        /// <returns>True if all items have been added successfully, false otherwise</returns>
        /// <remarks>The operation returns false when the graph already contains edges
        /// associated with one or more of the items</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Add(ReadOnlySpan<Char32> items)
        {
            bool result = false;
            for (UInt32 i = 0, current = 0; i < items.Length; i++)
            {
                result |= Add((int)i, ref current, items[(int)i]);
            }
            return result;
        }
        /// <summary>
        /// Extends this graph by the collection of items
        /// </summary>
        /// <param name="items">A collection of items to add to this graph</param>
        /// <returns>True if all items have been added successfully, false otherwise</returns>
        /// <remarks>The operation returns false when the graph already contains edges
        /// associated with one or more of the items</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Add(ReadOnlySpan<char> items)
        {
            bool result = false;
            for (UInt32 i = 0, current = 0; i < items.Length; i++)
            {
                result |= Add((int)i, ref current, items[(int)i]);
            }
            return result;
        }
        bool Add(int slice, ref UInt32 current, UInt32 item)
        {
            TokenGraphEdge edge = new TokenGraphEdge(slice, current, item);
            current = edge.Outgoing;
            
            int hash = edge.GetHashCode();
            if (!edges.Find(edge, hash, out int index, out int distance, out var result))
            {
                edges.Emplace(edge, hash, index, distance, edges.Version) = new HashSet.DefaultContainer<TokenGraphEdge>
                (
                    hash,
                    edge
                );
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Removes the edges of this graph
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            edges.Clear();
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            edges.Dispose();
        }

        /// <summary>
        /// Determines if the current edge has successor nodes
        /// </summary>
        /// <param name="current">The current edge to get successor nodes from</param>
        /// <returns>True if the slice exists, false otherwise</returns>
        [Pure]
        public bool HasSuccessor(TokenGraphEdge current)
        {
            int index = current.Slice + 1;
            foreach (TokenGraphEdge edge in edges)
            {
                if (edge.Slice == index && edge.Incoming == current.Outgoing)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Tries to obtain a slice at the provided index
        /// </summary>
        /// <param name="index">The slice index to obtain</param>
        /// <param name="slice">A collection to fill with the slice data</param>
        /// <returns>True if the slice exists, false otherwise</returns>
        [Pure]
        public bool TryGet(int index, ICollection<TokenGraphEdge> slice)
        {
            slice.Clear();
            foreach (TokenGraphEdge edge in edges)
            {
                if (edge.Slice == index)
                {
                    slice.Add(edge);
                }
            }
            return (slice.Count > 0);
        }
        /// <summary>
        /// Tries to obtain a slice at the provided index
        /// </summary>
        /// <param name="index">The slice index to obtain</param>
        /// <param name="slice">A collection to fill with the slice data</param>
        /// <returns>True if the slice exists, false otherwise</returns>
        [Pure]
        public bool TryGet<TArrayBuffer>(int index, ref EmbeddedList<TokenGraphEdge, TArrayBuffer> slice) 
            where TArrayBuffer : struct, IArrayAccessor<TokenGraphEdge>
        {
            slice.Clear();
            foreach (TokenGraphEdge edge in edges)
            {
                if (edge.Slice == index)
                {
                    slice.Add(edge);
                }
            }
            return (slice.Count > 0);
        }
        /// <summary>
        /// Tries to obtain the successor nodes of the provided edge
        /// </summary>
        /// <param name="current">The current edge to get successor nodes from</param>
        /// <param name="slice">A collection to fill with the slice data</param>
        /// <returns>True if the slice exists, false otherwise</returns>
        [Pure]
        public bool TryGet(TokenGraphEdge current, ICollection<TokenGraphEdge> slice)
        {
            slice.Clear();
            int index = current.Slice + 1;
            foreach (TokenGraphEdge edge in edges)
            {
                if (edge.Slice == index && edge.Incoming == current.Outgoing)
                {
                    slice.Add(edge);
                }
            }
            return (slice.Count > 0);
        }
        /// <summary>
        /// Tries to obtain the successor nodes of the provided edge
        /// </summary>
        /// <param name="current">The current edge to get successor nodes from</param>
        /// <param name="slice">A collection to fill with the slice data</param>
        /// <returns>True if the slice exists, false otherwise</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet<TArrayBuffer>(TokenGraphEdge current, ref EmbeddedList<TokenGraphEdge, TArrayBuffer> slice) 
            where TArrayBuffer : struct, IArrayAccessor<TokenGraphEdge>
        {
            slice.Clear();
            int index = current.Slice + 1;
            foreach (TokenGraphEdge edge in edges)
            {
                if (edge.Slice == index && edge.Incoming == current.Outgoing)
                {
                    slice.Add(edge);
                }
            }
            return (slice.Count > 0);
        }
    }
}
