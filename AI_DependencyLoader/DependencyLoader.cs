using BepInEx;
using IL_DependencyLoader;
using static BepInEx.Harmony.HarmonyWrapper;

namespace AI_DependencyLoader
{
    [BepInPlugin(Guid, "AI_" + PluginInformation.ReadableName, PluginInformation.Version)]
    public class DependencyLoader : BaseUnityPlugin
    {
        private const string Guid = PluginInformation.CommonPrefix + ".ai." + PluginInformation.Name;

        private void Awake()
        {
            PatchAll(typeof(GameHooks));
        }
    }
}