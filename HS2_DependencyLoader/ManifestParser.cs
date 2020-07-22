using System.Collections.Generic;
using System.Linq;
using IL_DependencyLoader;

namespace HS2_DependencyLoader
{
    // This is for some bullshits that does not contains manifest information in the list file.
    // such as HS2 Main Map Systems. Fuck that shit.
    public static class ManifestInfo
    {
        private static readonly Dictionary<string, Dictionary<string, string>> Data = new Dictionary<string, Dictionary<string, string>>();
        private static readonly List<string> Manifests = new List<string>();

        public static bool Get(string bundle, string asset, out string result)
        {
            result = null;
            if (!Data.TryGetValue(bundle, out var assetDict) ||
                assetDict == null ||
                !assetDict.TryGetValue(asset, out var manifest) ||
                Cursed.IsCursedManifest(manifest)) return false;

            result = manifest;
            return true;
        }

        public static void Add(string bundle, string asset, string manifest)
        {
            if (!Data.ContainsKey(bundle)) Data[bundle] = new Dictionary<string, string>();
            Data[bundle][asset] = manifest;
        }


        public static void AddManifest(string manifest)
        {
            Manifests.Add(manifest); // all of custom manifests will have unity3d after it.
        }

        public static List<string> GetManifests()
        {
            return Manifests.Distinct().ToList();
        }
    }

    public static class ManifestParser
    {
        public static void ParseXmlManifests()
        {
            foreach (var document in Sideloader.Sideloader.Manifests.Values.Select(x => x.manifestDocument))
            {
                var manifests = document?.Root?.Element("hs2-scene-dependency") ?? document?.Root?.Element("scene-dependency");
                if (manifests == null) continue;
                var manifestFile = manifests.Attribute("manifest")?.Value;
                if (manifestFile.IsNullOrEmpty() || manifestFile.IsNullOrWhiteSpace()) continue;
                ManifestInfo.AddManifest(manifestFile);
                foreach (var manifest in manifests.Elements("dependency"))
                {
                    var bundle = manifest.Attribute("bundle")?.Value;
                    var asset = manifest.Attribute("asset")?.Value;
                    if (bundle.IsNullOrEmpty() || bundle.IsNullOrWhiteSpace() || asset.IsNullOrEmpty() || asset.IsNullOrWhiteSpace()) continue;
                    ManifestInfo.Add(bundle, asset, manifestFile);
                }
            }
        }
    }
}