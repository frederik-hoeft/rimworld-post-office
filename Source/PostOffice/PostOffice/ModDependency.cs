using PostOffice.Dependencies;
using System.Linq;
using Verse;

namespace PostOffice;

internal static class ModDependency
{
    public const string CAI5000 = "Krkr.rule56";

    private static bool? s_isCai5000Loaded;

    public static bool IsCai5000Loaded => s_isCai5000Loaded ??= IsAvailable(CAI5000);

    public static bool IsAvailable(string modId) =>
        LoadedModManager.RunningMods.Any(mod =>
            mod.PackageIdPlayerFacing?.Equals(modId) is true);

    public static bool IsSatisfied<TRequiresMod>() where TRequiresMod : struct, IRequiresMod =>
        default(TRequiresMod).ModId is string modId && IsAvailable(modId);
}
