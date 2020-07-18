using BepInEx;
using IL_DependencyLoader;
using KKAPI;
using KKAPI.Studio;
using static BepInEx.Harmony.HarmonyWrapper;

namespace AI_DependencyLoader
{
    [BepInDependency(KoikatuAPI.GUID)]
    [BepInPlugin(Guid, "AI_DependencyLoader", Version)]
    public class DependencyLoader : BaseUnityPlugin
    {
        private const string Guid = "com.hooh.ai.deploader";
        private const string Version = "1.0.0";

        private void Awake()
        {
            if (!StudioAPI.InsideStudio) return;
            PatchAll(typeof(GameHooks));

            GameHooks.Logger = Logger;
            Dependency.Logger = Logger;
        }
    }
}