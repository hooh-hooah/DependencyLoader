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
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public static void LoadMap(Map __instance, int _no)
        {
            if (__instance.no == _no ||
                !Singleton<Info>.Instance.dicMapLoadInfo.TryGetValue(_no, out var data) ||
                Cursed.IsCursedManifest(data.manifest)) return;
            Dependency.LoadDependency(data.bundlePath, data.manifest);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Map), nameof(Map.LoadMapCoroutine), typeof(int), typeof(bool))]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        // ReSharper disable once UnusedMember.Global
        public static void LoadMap(Map __instance, int _no, bool _wait)
        {
            LoadMap(__instance, _no);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AddObjectItem), "GetLoadInfo")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        // ReSharper disable once UnusedMember.Global
        public static void GetLoadInfo(int _group, int _category, int _no)
        {
            if (!Singleton<Info>.Instance.dicItemLoadInfo.TryGetValue(_group, out var dictionary) ||
                !dictionary.TryGetValue(_category, out var dictionary2) ||
                !dictionary2.TryGetValue(_no, out var result) ||
                Cursed.IsCursedManifest(result.manifest)) return;
            Dependency.LoadDependency(result.bundlePath, result.manifest);
        }
    }
}