using BepInEx;
using BepInEx.Harmony;
using IL_DependencyLoader;

namespace HS2_DependencyLoader
{
    [BepInPlugin(Guid, "HS2_" + PluginInformation.ReadableName, PluginInformation.Version)]
    public class DependencyLoader : BaseUnityPlugin
    {
        private const string Guid = PluginInformation.CommonPrefix + ".hs2." + PluginInformation.Name;

        private void Awake()
        {
            ManifestParser.ParseXmlManifests();
            HarmonyWrapper.PatchAll(typeof(GameHooks));
        }
    }
}