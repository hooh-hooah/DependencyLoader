using System.Diagnostics.CodeAnalysis;
using ADV.EventCG;
using BepInEx.Logging;
using HarmonyLib;
using IL_DependencyLoader;
using Manager;
using Studio;
using UnityEngine;

namespace AI_DependencyLoader
{
    public class GameHooks
    {
	    [HarmonyPrefix, HarmonyPatch(typeof(Studio.Map), nameof(Studio.Map.LoadMap))]
        // [SuppressMessage("ReSharper", "InconsistentNaming")]
        // // ReSharper disable once UnusedMember.Global
		public static bool LoadMap(Studio.Map __instance, int _no)
		{
			if (!Singleton<Info>.Instance.dicMapLoadInfo.ContainsKey(_no))
			{
				__instance.ReleaseMap();
				return false;
			}
			if (__instance.no == _no) return false;

			__instance.PrivateSetter("MapComponent", null);
			__instance.PrivateSetter("isLoading", true);
			__instance.PrivateSetter("no", _no);
			
			var data = Singleton<Info>.Instance.dicMapLoadInfo[_no];
			if (!CursedList.IsCursedManifest(data.manifest))
			{
				Dependency.LoadDependency(data.bundlePath, data.manifest);
			}
			
			Singleton<Scene>.Instance.LoadBaseScene(new Scene.Data
			{
				assetBundleName = data.bundlePath,
				levelName = data.fileName,
				fadeType = Scene.Data.FadeType.None,
				onLoad = delegate
				{
					__instance.InvokeMethod("OnLoadAfter", data.fileName);
				}
			});
			return false;
		}
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AddObjectItem), "GetLoadInfo")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        // ReSharper disable once UnusedMember.Global
        public static void GetLoadInfo(int _group, int _category, int _no)
        {
            if (!Singleton<Info>.Instance.dicItemLoadInfo.TryGetValue(_group, out var dictionary))
                return;
            if (!dictionary.TryGetValue(_category, out var dictionary2))
                return;
            if (!dictionary2.TryGetValue(_no, out var result))
                return;

            Dependency.LoadDependency(result.bundlePath, result.manifest);
        }

        public static ManualLogSource Logger { get; set; }
    }
}