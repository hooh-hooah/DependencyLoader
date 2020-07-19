using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using IL_DependencyLoader;
using Manager;

namespace HS2_DependencyLoader
{
    // This is for some bullshits that does not supports manifests.
    public static class ManifestInfo
    {
        private static readonly Dictionary<string, Dictionary<string, string>> Data = new Dictionary<string, Dictionary<string, string>>();

        public static bool Get(string bundle, string asset, out string result)
        {
            result = null;
            Data.TryGetValue(bundle, out var assetDict);
            if (assetDict == null) return false;
            if (!assetDict.TryGetValue(asset, out var manifest)) return false;
            if (manifest.IsNullOrEmpty() || manifest.IsNullOrWhiteSpace()) return false;
            if (Cursed.IsCursedManifest(manifest)) return false;
            
            result = manifest;
            return true;
        }
        
        public static void Add(string bundle, string asset, string manifest)
        {
            if (!Data.ContainsKey(bundle)) Data[bundle] = new Dictionary<string, string>();
            Data[bundle][asset] = manifest;
        }
    }
    public static class ManifestParser
    {
        public static void ParseXmlManifests()
        {
            foreach (var document in Sideloader.Sideloader.Manifests.Values.Select(x => x.manifestDocument))
            {
                var manifests = document?.Root?.Element("hs2-scene-dependency");
                if (manifests == null) continue;
                var manifestFile = manifests.Attribute("manifest")?.Value;
                if (manifestFile.IsNullOrEmpty() || manifestFile.IsNullOrWhiteSpace()) continue;
                foreach (var manifest in manifests.Elements("dependency"))
                {
                    var bundle = manifest.Attribute("bundle")?.Value;
                    var asset = manifest.Attribute("asset")?.Value;
                    if (bundle.IsNullOrEmpty() || bundle.IsNullOrWhiteSpace() || asset.IsNullOrEmpty() || asset.IsNullOrWhiteSpace()) continue;
                    ManifestInfo.Add(bundle, asset, manifestFile);
                    Logger.LogError($"Registered {manifestFile}>{bundle}>{asset}");
                }
            }
        }

        public static ManualLogSource Logger { get; set; }
    }
}