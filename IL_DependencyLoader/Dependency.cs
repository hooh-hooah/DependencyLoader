using System.Linq;
using UnityEngine;

namespace IL_DependencyLoader
{
    public static class Dependency
    {
        private const string ManifestAssetName = "AssetBundleManifest";

        private static bool GetManifest(string manifestPath, out AssetBundleManifest manifest)
        {
            manifest = null;
            if (manifestPath.IsNullOrEmpty() || manifestPath.IsNullOrWhiteSpace()) return false;

            var bundle = AssetBundleManager
                .LoadAsset(
                    $"{manifestPath}.unity3d",
                    ManifestAssetName,
                    typeof(AssetBundleManifest),
                    ""
                );
            if (bundle.IsEmpty()) return false;

            var asset = bundle.GetAsset<AssetBundleManifest>();
            if (asset == null) return false;

            manifest = asset;
            return true;
        }

        private static void LoadDependencyAssetBundles(string bundleName, AssetBundleManifest asset)
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
            if (GetManifest(manifestName, out var manifestAsset)) LoadDependencyAssetBundles(bundlePath, manifestAsset);
        }
    }
}