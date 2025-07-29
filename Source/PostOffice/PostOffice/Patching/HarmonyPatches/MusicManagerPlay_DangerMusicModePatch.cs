// conditional compilation patch for CAI-5000

#if CAI5000

using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;
using CombatAI;

namespace PostOffice.Patching.HarmonyPatches;

[HarmonyPatch(typeof(MusicManagerPlay), nameof(MusicManagerPlay.DangerMusicMode), MethodType.Getter)]
[RequiresMod(ModDependency.CAI5000)]
public static class MusicManagerPlay_DangerMusicModePatch
{
    private static bool s_previousResult;

    public static bool Prefix(MusicManagerPlay __instance, ref bool __result)
    {
        // override base game implementation only iff mod is enabled
        // and dependencies are loaded.
        if (PostOfficeMod.Settings is { isActive: true, cai5000_delayCombatMusic: true} && ModDependency.IsCai5000Loaded)
        {
            // for every map check what the game thinks about the danger rating
            bool resultSet = false;
            List<Map> maps = Find.Maps;
            for (int index = 0; index < maps.Count; index++)
            {
                if (maps[index].dangerWatcher.DangerRating == StoryDanger.High)
                {
                    // do some additional checks and only play combat music iff
                    // enemies are *actually visible*
                    Map map = maps[index];
                    // DO NOT capture this variable in any lambda or closure! There are reports of other mods using reflection to enumerate all fields of all types in the assembly,
                    // and this will cause a crash if it attempts to enumerate the captured MapComponent_FogGrid instance if CAI-5000 is not loaded.
                    // (even though, personally, I would blame the other authors for doing that, not me. Touch something you shouldn't, and you get burned.)
                    // but yeah, anyways, for the sake of compatibility, don't capture this variable.
                    MapComponent_FogGrid? fog = map.GetComp_Fast<MapComponent_FogGrid>();
                    if (fog is not null)
                    {
                        // check if there are any visible threats (DO NOT use LINQ here, see above)
                        bool foundVisibleThreat = false;
                        for (int i = 0; i < map.mapPawns.AllPawnsSpawned.Count; i++)
                        {
                            Pawn pawn = map.mapPawns.AllPawnsSpawned[i];
                            if (pawn.HostileTo(Faction.OfPlayerSilentFail) && !fog.IsFogged(pawn.Position))
                            {
                                foundVisibleThreat = true;
                                break;
                            }
                        }
                        __result = foundVisibleThreat;
                        resultSet = true;
                        if (__result)
                        {
                            // prevent spamming the log
                            if (s_previousResult != __result)
                            {
                                Logger.LogVerbose($"(CAI-5000 patch) DangerRating is high and RimWorld is attempting to play combat music! Threat is visible, so allowing combat music.");
                            }
                            s_previousResult = __result;
                            // if we already found a visible threat, we can stop here
                            return false;
                        }
                        // prevent spamming the log
                        else if (s_previousResult != __result)
                        {
                            s_previousResult = __result;
                            Logger.LogVerbose($"(CAI-5000 patch) DangerRating is high and RimWorld is attempting to play combat music! Threat is not visible, so blocking combat music.");
                        }
                    }
                    else
                    {
                        // if fog of war is disabled or otherwise unavailable, allow default behavior (no need to continue checking)
                        __result = true;
                        return false;
                    }
                }
            }
            if (!resultSet)
            {
                // we didn't find any high danger maps.
                // the base game allows danger music to play if override is enabled.
                __result = __instance.OverrideDangerMode;
            }
            // no need to call into base game implementation (we basically did everything here)
            return false;
        }
        // mod is disabled or dependencies are not loaded
        // forward to base game implementation
        return true;
    }
}

#endif // CAI5000