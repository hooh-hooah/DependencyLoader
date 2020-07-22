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
        [HarmonyPatch(typeof(BaseMap), nameof(BaseMap.ChangeAsync), typeof(int), typeof(FadeCanvas.Fade), typeof(bool))]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        // ReSharper disable once UnusedMember.Global
        public static void ChangeAsync(int _no, FadeCanvas.Fade fadeType = FadeCanvas.Fade.InOut, bool isForce = false)
        {
            if (!(ReferenceEquals(BaseMap.mapRoot, null) || BaseMap.no != _no || isForce) ||
                !BaseMap.infoTable.TryGetValue(_no, out var info) ||
                info == null ||
                !ManifestInfo.Get(info.AssetBundleName, info.AssetName, out var manifest) ||
                Cursed.IsCursedManifest(manifest)) return;
            Dependency.LoadDependency(info.AssetBundleName, manifest);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Studio.Map), nameof(Studio.Map.LoadMap), typeof(int))]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        // ReSharper disable once UnusedMember.Global
        public static void LoadMap(Studio.Map __instance, int _no)
        {
            if (__instance.no == _no ||
                !Singleton<Info>.Instance.dicMapLoadInfo.TryGetValue(_no, out var data) ||
                Cursed.IsCursedManifest(data.manifest)) return;
            Dependency.LoadDependency(data.bundlePath, data.manifest);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Studio.Map), nameof(Studio.Map.LoadMapCoroutine), typeof(int), typeof(bool))]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        // ReSharper disable once UnusedMember.Global
        public static void LoadMap(Studio.Map __instance, int _no, bool _wait)
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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AssetBundleManager), nameof(AssetBundleManager.Initialize))]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        // ReSharper disable once UnusedMember.Global
        public static void InitializeAssetBundleManager()
        {
            ManifestInfo.GetManifests().ForEach(Dependency.AddAssetBundleManifest);
        }
    }
}