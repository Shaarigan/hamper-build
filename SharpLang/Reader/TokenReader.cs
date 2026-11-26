// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

using System.Runtime.CompilerServices;
using System.Text;
using Soe.Collections.Embedded;
using Soe.Parsing;
using Soe.Pooling;

namespace Soe.Build.SharpLang
{
    /// <summary>
    /// A stream reader specialized to process C-Sharp code into parser tokens
    /// </summary>
    #if EXPORT_HAMPER_BUILD_SHARPLANG
    public
    #else
    internal
    #endif
    partial class TokenReader<Source> : TrackedTokenReader<SharpToken, Source>, ITextProvider<TextBuffer>
        where Source : struct, IStreamSource
    {
        private TextBuffer buffer;
        
        private TextRange match;
        /// <summary>
        /// The text range of current token
        /// </summary>
        public TextRange Match
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return match; }
        }

        private EmbeddedList<string, PoolArray<string>> errors;
        /// <summary>
        /// Gets a collection of error messages appeared while parsing JSON
        /// </summary>
        public ReadOnlySpan<string> Errors
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return errors.AsSpan(); }
        }
        
        /// <inheritdoc/>
        protected override bool AutoAdvanceCaret
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return true; }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TokenReader(Source source)
            : base(source)
        {
            this.buffer = new TextBuffer(StringBuilderPool.Shared.Rent());
            this.errors = new EmbeddedList<string, PoolArray<string>>();
        }
        
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Initialize(Source source)
        {
            base.Initialize(source);
            this.buffer.Clear(true);
            this.errors.Clear();
        }
        
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Dispose()
        {
            errors.Dispose();
            StringBuilderPool.Shared.Return(buffer.Buffer);
            buffer = default;
        }
        
        #region ITextProvider Members
        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TextSource<TextBuffer> ITextProvider<TextBuffer>.GetSource(int startIndex, int length)
        {
            return new TextSource<TextBuffer>(ref buffer, startIndex, length);
        }
        #endregion
    }
}