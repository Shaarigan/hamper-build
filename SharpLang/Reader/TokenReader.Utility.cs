// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

using System.Runtime.CompilerServices;
using Soe.Unicode;

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
    partial class TokenReader<Source>
    {
        /// <summary>
        /// Determines if a character is a valid identifier character to this tokenizer
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIdentifierChar(Char32 c)
        {
            return ((c >= Scripting.LowercaseLetter.Begin && c <= Scripting.LowercaseLetter.End) || 
                    (c >= Scripting.UppercaseLetter.Begin && c <= Scripting.UppercaseLetter.End) || 
                    (c >= Scripting.Digit.Begin && c <= Scripting.Digit.End) || 
                     c == Scripting.Punctuation.LowLine);
        }
    }
}