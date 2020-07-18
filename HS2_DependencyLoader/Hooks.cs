using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BepInEx.Logging;
using HarmonyLib;
using IL_DependencyLoader;
using Manager;
using Studio;
using UniRx.Async;

namespace HS2_DependencyLoader
{
    public class GameHooks
    {
	    // Main Game Map Dependency Support
	    
	    /*[HarmonyPrefix]
	    [HarmonyPatch(typeof(BaseMap), nameof(BaseMap.ChangeAsync))]
	    [SuppressMessage("ReSharper", "InconsistentNaming")]
	    // ReSharper disable once UnusedMember.Global
	    public static async UniTask ChangeAsync(int _no, FadeCanvas.Fade fadeType = FadeCanvas.Fade.InOut, bool isForce = false)
	    {
		    if (!(BaseMap.mapRoot != null) || BaseMap.no != _no || isForce)
		    {
			    typeof(BaseMap).SetStaticField("isMapLoading", true);
			    typeof(BaseMap).SetStaticField("prevNo", BaseMap.no);
			    typeof(BaseMap).SetStaticField("no", _no);
				MapInfo.Param info = BaseMap.Info;
				await Scene.LoadBaseSceneAsync(new Scene.Data
				{
					bundleName = info.AssetBundleName,
					levelName = info.AssetName,
					// what the fuck?
					fadeType = fadeType
				});
			}
	    }*/
        
        [HarmonyPrefix, HarmonyPatch(typeof(Studio.Map), nameof(Studio.Map.LoadMap), typeof(int))]
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
			
			
			Info.MapLoadInfo data = Singleton<Info>.Instance.dicMapLoadInfo[_no];

			if (!CursedList.IsCursedManifest(data.manifest))
			{
				Dependency.LoadDependency(data.bundlePath, data.manifest);
			}
			
			// i tried to use manifest option for this one but apparently it's not working so 
			// it's basically huge fuck off from ILLUSION
			Scene.LoadBaseScene(new Scene.Data
			{
				bundleName = data.bundlePath,
				levelName = data.fileName,
				fadeType = FadeCanvas.Fade.None,
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