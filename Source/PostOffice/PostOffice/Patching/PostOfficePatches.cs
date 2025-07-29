using HarmonyLib;
using System.Linq;
using System.Reflection;

namespace PostOffice.Patching;

internal static class PostOfficePatches
{
    public static void Apply(Harmony harmony)
    {
        Logger.LogAlways($"applying patches...");
        Type[] harmonyPatches = 
        [
            .. typeof(PostOfficePatches).Assembly.GetTypes().Where(static t => t is 
            {
                // static class <==> class && sealed && abstract
                IsClass: true,
                IsAbstract: true,
                IsSealed: true
            } && t.TryGetAttribute<HarmonyPatch>(out _))
        ];

        int patchCount = 0;
        foreach (Type harmonyPatch in harmonyPatches)
        {
            if (harmonyPatch.TryGetAttribute(out RequiresModAttribute? dependency) && !ModDependency.IsAvailable(dependency.ModId))
            {
                Logger.LogAlways($"skipping {harmonyPatch.Name} due to missing dependency: '{dependency.ModId}'");
                continue;
            }
            harmony.CreateClassProcessor(harmonyPatch).Patch();
            Logger.LogAlways($"applied {harmonyPatch.Name}{(dependency is not null ? $" because {dependency.ModId} was detected" : string.Empty)}");
            patchCount++;
        }
        Logger.LogAlways($"applied {patchCount} patches!");
    }

    private static bool TryGetAttribute<TAttribute>(this Type type, [NotNullWhen(returnValue: true)] out TAttribute? attribute) 
        where TAttribute : Attribute
    {
        attribute = type.GetCustomAttribute<TAttribute>();
        return attribute is not null;
    }
}
