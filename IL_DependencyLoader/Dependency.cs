using System.Linq;
using System.Text.RegularExpressions;
using BepInEx.Logging;
using UnityEngine;

namespace IL_DependencyLoader
{
    public static class Dependency
    {
        private const string ManifestAssetName = "AssetBundleManifest";
        public static ManualLogSource Logger { get; set; }

        private static AssetBundleManifest GetManifest(string manifestPath)
        {
            var bundlePath = $"{manifestPath}.unity3d";
            var bundle = AssetBundleManager.LoadAsset(bundlePath, ManifestAssetName, typeof(AssetBundleManifest), "");
            if (bundle.IsEmpty()) return null;

            var manifest = bundle.GetAsset<AssetBundleManifest>();
            if (manifest == null) return null;

            return manifest;
        }

        private static void LoadDependency(string bundleName, string manifestName, AssetBundleManifest asset)
        {
            asset.GetAllDependencies(bundleName).ToList().ForEach(depBundle =>
            {
                #if HS2
                    AssetBundleManager.LoadAssetBundle(depBundle);
                #else
                    AssetBundleManager.LoadAssetBundle(depBundle, false);
                #endif
            });
        }

        public static void LoadDependency(string bundlePath, string manifestName)
        {
            Logger.LogDebug($"Trying to load dependency of {bundlePath}");
            if (manifestName.IsNullOrEmpty() || manifestName.IsNullOrWhiteSpace()) return;

            var manifestAsset = GetManifest(manifestName);
            if (manifestAsset == null) return;

            LoadDependency(bundlePath, manifestName, manifestAsset);
        }
    }
}