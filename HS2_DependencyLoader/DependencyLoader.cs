using BepInEx;
using KKAPI;
using KKAPI.Studio;
using static BepInEx.Harmony.HarmonyWrapper;

namespace HS2_DependencyLoader
{
    [BepInDependency(KoikatuAPI.GUID)]
    [BepInPlugin(Guid, "HS2_DependencyLoader", Version)]
    public class DependencyLoader : BaseUnityPlugin
    {
        private const string Guid = "com.hooh.hs2.deploader";
        private const string Version = "1.0.0";

        public DependencyLoader Instance { get; set; }

        private void Awake()
        {
            Instance = this;
            if (!StudioAPI.InsideStudio) return;
            PatchAll(typeof(GameHooks));
        }
    }
}