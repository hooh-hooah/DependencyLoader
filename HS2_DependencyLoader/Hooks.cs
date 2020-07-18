using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using IL_DependencyLoader;
using Manager;
using Studio;

namespace HS2_DependencyLoader
{
    public class GameHooks
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Scene.Data), nameof(Scene.Data.Load))]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        // ReSharper disable once UnusedMember.Global
        public static void SceneLoad(Scene.Data __instance)
        {
            Dependency.LoadDependency(__instance.bundleName);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AddObjectItem), "GetLoadInfo")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        // ReSharper disable once UnusedMember.Global
        public static void GetLoadInfo(int _group, int _category, int _no)
        {
	        if (!Singleton<Info>.Instance.dicVoiceLoadInfo.TryGetValue(_group, out var dictionary))
				return;
	        if (!dictionary.TryGetValue(_category, out var dictionary2))
				return;
	        if (!dictionary2.TryGetValue(_no, out var result))
				return;
            Dependency.LoadDependency(result.bundlePath);
        }
    }
}