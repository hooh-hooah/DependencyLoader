using BepInEx;
using BepInEx.Harmony;
using IL_DependencyLoader;
using KKAPI;

namespace HS2_DependencyLoader
{
    [BepInDependency(KoikatuAPI.GUID)]
    [BepInPlugin(Guid, "HS2_DependencyLoader", Version)]
    public class DependencyLoader : BaseUnityPlugin
    {
        private const string Guid = "com.hooh.hs2.deploader";
        private const string Version = "1.0.0";

        private void Awake()
        {
            GameHooks.Logger = Logger;
            ManifestParser.Logger = Logger;
            Dependency.Logger = Logger;
            
            ManifestParser.ParseXmlManifests();
            HarmonyWrapper.PatchAll(typeof(GameHooks));
        }
    }
}