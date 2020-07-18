using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace IL_DependencyLoader
{
    public class Dependency
    {
        private const string ManifestAssetName = "AssetBundleManifest";
        private const string PathSplitPattern = @"[\\\/]";

        private static string GuessManifestName(string bundlePath)
        {
            if (bundlePath.IsNullOrEmpty() || bundlePath.IsNullOrWhiteSpace()) return null;

            var possibleManifest = Regex.Split(bundlePath, PathSplitPattern).FirstOrDefault(x => x.Length > 0);
            if (possibleManifest.IsNullOrEmpty() || possibleManifest.IsNullOrWhiteSpace()) return null;

            return possibleManifest;
        }

        private static AssetBundleManifest GetManifest(string manifestPath)
        {
            var bundlePath = $"{manifestPath}/{manifestPath}.unity3d";
            var bundle = AssetBundleManager.LoadAsset(bundlePath, ManifestAssetName, typeof(AssetBundleManifest), "");
            if (bundle.IsEmpty()) return null;

            var manifest = bundle.GetAsset<AssetBundleManifest>();
            if (manifest == null) return null;

            return manifest;
        }

        private static void LoadDependency(string bundleName, string manifestName, AssetBundleManifest asset)
        {
            var manifestPathAdjustment = $"{manifestName}/";
            asset.GetAllDependencies(bundleName.Replace(manifestPathAdjustment, "")).ToList().ForEach(depBundle =>
            {
                #if HS2
                    AssetBundleManager.LoadAssetBundle(manifestPathAdjustment + depBundle);
                #else
                    AssetBundleManager.LoadAssetBundle(manifestPathAdjustment + depBundle, false);
                #endif
            });
        }

        public static void LoadDependency(string bundlePath)
        {
            var possibleManifest = GuessManifestName(bundlePath);
            if (possibleManifest.IsNullOrEmpty() || possibleManifest.IsNullOrWhiteSpace()) return;

            var manifestAsset = GetManifest(possibleManifest);
            if (manifestAsset == null) return;

            LoadDependency(bundlePath, possibleManifest, manifestAsset);
        }
    }
}