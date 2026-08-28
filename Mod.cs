using HarmonyLib;
using KitchenLib;
using KitchenLib.Interfaces;
using KitchenMods;
using System.Reflection;
using UnityEngine;
using KitchenLogger = KitchenLib.Logging.KitchenLogger;

namespace TestMod
{
    public class Mod : BaseMod, IModSystem, IAutoRegisterAll // IAutoRegisterAll does all of the CustomItems, Groups<T> and CustomDishes
    {
        /*
         * GUID: A unique identifier for your mod. This should be unique to your mod. Once you set it, do not change it.
         * NAME: The name of your mod. This is what will be displayed in the mod manager.
         * VERSION: The version of your mod. This is what will be displayed in the mod manager.
         * AUTHOR: Your name.
         * GAMEVERSION: The version of the game that your mod is compatible with. This is uses Semantic Versioning which can be found here: https://semver.org/
         */
        public const string MOD_GUID = "com.pinemoose.testMod";
        public const string MOD_NAME = "Test Mod";
        public const string MOD_VERSION = "0.1";
        public const string MOD_AUTHOR = "PineMoose";
        public const string MOD_GAMEVERSION = ">=1.3.0";

        private static bool _harmonyPatched = false;

        /*
         * Bundle: This is the asset bundle that contains all of your mod's assets.
         * Logger: This is the logger that you will use to log information to the console.
         */
        internal static AssetBundle Bundle;
        internal static KitchenLogger Logger;

        /*
         * Mod Constructor This is where you will set the GUID, NAME, VERSION, AUTHOR, and GAMEVERSION for your mod.
         */
        public Mod() : base(
                MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, 
                Assembly.GetExecutingAssembly()
            ) { }

        /*
         * OnInitialise method. This is called when the user loads into the lobby.
         */
        protected override void OnInitialise()
        {
            Logger.LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
        }

        /*
         * OnUpdate method. This is called every frame after OnInitialise().
         */
        protected override void OnUpdate() { }

        /*
         * OnPostActivate method. This is called after the mod is activated.
         */
        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
            //  Bundle = mod.GetPacks<AssetBundleModPack>()
            //      .SelectMany(e => e.AssetBundles)
            //      .FirstOrDefault() ?? throw new MissingAssetBundleException(MOD_GUID);

            Logger = InitLogger();
            Mod.Logger.LogInfo("TestMod loaded successfully!");
            if (!_harmonyPatched)
            {
                var harmony = new Harmony(MOD_GUID);
                harmony.PatchAll();
                _harmonyPatched = true;

                foreach (var method in harmony.GetPatchedMethods())
                {
                    Mod.Logger.LogInfo($"[DEBUGGING] Patched method: {method.DeclaringType}.{method.Name}");
                }
            }
        }
    }
}