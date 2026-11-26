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
    partial class TokenReader<Source>
    {
        protected override bool TryRead(out SharpToken token)
        {
            throw new NotImplementedException();
        }
    }
}