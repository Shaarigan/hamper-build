// Licensed to Schroedinger Entertainment (SOE) under the terms of the AGPLv3
// Licensed to you by SOE under the terms of the AGPLv3 or another OSI-approved license 

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
    enum SharpToken : byte
    {
        Invalid = 0,

        #region Reserved keywords
        Abstract,
        As,
        Base,
        Bool,
        Break,
        Byte,
        Case,
        Catch,
        Char,
        Checked,
        Class,
        Const,
        Continue,
        Decimal,
        Default,
        Delegate,
        Do,
        Double,
        Else,
        Enum,
        Event,
        Explicit,
        Extern,
        False,
        Finally,
        Fixed,
        Float,
        For,
        Foreach,
        Goto,
        If,
        Implicit,
        In,
        Int,
        Interface,
        Internal,
        Is,
        Lock,
        Long,
        Namespace,
        New,
        Null,
        Object,
        Operator,
        Out,
        Override,
        Params,
        Private,
        Protected,
        Public,
        Readonly,
        Ref,
        Return,
        Sbyte,
        Sealed,
        Short,
        Sizeof,
        Stackalloc,
        Static,
        String,
        Struct,
        Switch,
        This,
        Throw,
        True,
        Try,
        Typeof,
        Uint,
        Ulong,
        Unchecked,
        Unsafe,
        Ushort,
        Using,
        Virtual,
        Void,
        Volatile,
        While,
        #endregion
        
        #region Preprocessor directives
        Define,
        Undefine,
        IfDirective,
        ElifDirective,
        ElseDirective,
        EndifDirective,
        Error,
        Warning,
        Region,
        Endregion,
        Line,
        Nullable,
        Pragma,
        #endregion
    }
}