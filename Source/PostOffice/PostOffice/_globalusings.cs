global using System;
global using Logger = PostOffice.Logging.Logger;
global using Std = System;
global using System.Diagnostics.CodeAnalysis;
global using static PostOffice.Globals;
global using static PostOffice.Roslyn.BuildIntrinsics.Suppressions;

namespace PostOffice;

public static class Globals
{
    /// <summary>
    /// A typed null value to be used for parameter-less switch expressions.
    /// </summary>
    public const object? __ = null;
}