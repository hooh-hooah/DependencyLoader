using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using IL_DependencyLoader;
using Studio;

namespace AI_DependencyLoader
{
    public class GameHooks
    {
        // TODO: Add AI-Syoujyo Map Support. 
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Map), nameof(Map.LoadMap))]
        // [SuppressMessage("ReSharper", "InconsistentNaming")]
        // ReSharper disable once UnusedMember.Global
        public static void LoadMap(Map __instance, int _no)
        {
            if (__instance.no == _no) return;
            if (!Singleton<Info>.Instance.dicMapLoadInfo.TryGetValue(_no, out var data)) return;
            if (Cursed.IsCursedManifest(data.manifest)) return;
            Dependency.LoadDependency(data.bundlePath, data.manifest);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AddObjectItem), "GetLoadInfo")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        // ReSharper disable once UnusedMember.Global
        public static void GetLoadInfo(int _group, int _category, int _no)
        {
            if (!Singleton<Info>.Instance.dicItemLoadInfo.TryGetValue(_group, out var dictionary)) return;
            if (!dictionary.TryGetValue(_category, out var dictionary2)) return;
            if (!dictionary2.TryGetValue(_no, out var result)) return;
            Dependency.LoadDependency(result.bundlePath, result.manifest);
        }
    }
}