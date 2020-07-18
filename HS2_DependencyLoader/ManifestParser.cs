using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using Manager;

namespace HS2_DependencyLoader
{
    // This is for some bullshits that does not supports manifests.
    public static class ManifestInfo
    {
        private static Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();

        public static string Get(string bundle, string asset)
        {
            if (data.ContainsKey(bundle) && data[bundle].ContainsKey(asset))
            {
                return data[bundle][asset];
            }

            return null;
        }
        
        public static void Add(string bundle, string asset, string manifest)
        {
            if (!data.ContainsKey(bundle)) data[bundle] = new Dictionary<string, string>();
            data[bundle][asset] = manifest;
        }
    }
    public static class ManifestParser
    {
       
        public static void ParseXMLManifests()
        {
            Logger.LogInfo("[Heelz] Heels mode activated: destroy all foot");
            foreach (var document in Sideloader.Sideloader.Manifests.Values.Select(x => x.manifestDocument))
            {
                var manifests = document?.Root?.Element("map-manifests");
                if (manifests == null) continue;
                var manifestFile = manifests.Attribute("manifest")?.Value;
                if (manifestFile.IsNullOrEmpty() || manifestFile.IsNullOrWhiteSpace()) continue;
                foreach (var manifest in manifests.Elements("manifest"))
                {
                    var bundle = manifest.Attribute("bundle")?.Value;
                    var asset = manifest.Attribute("name")?.Value;
                    if (bundle.IsNullOrEmpty() || bundle.IsNullOrWhiteSpace() || asset.IsNullOrEmpty() || asset.IsNullOrWhiteSpace()) continue;
                    ManifestInfo.Add(bundle, asset, manifestFile);
                }
            }
        }

        public static ManualLogSource Logger { get; set; }
    }
}