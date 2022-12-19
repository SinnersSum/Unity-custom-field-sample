using System.ComponentModel;

#nullable enable

#if !NET5_0_OR_GREATER

namespace System.Runtime.CompilerServices
{

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit { }
}

#endif