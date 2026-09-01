using Kitchen;
using KitchenLib;
using KitchenLib.Event;
using KitchenLib.Interfaces;
using KitchenLib.Preferences;
using KitchenLib.UI.PlateUp.PreferenceMenus;
using KitchenMods;
using PlateVsPlate.Setttings;
using System;
using System.Reflection;
using KitchenLogger = KitchenLib.Logging.KitchenLogger;

namespace PlateVsPlate
{
    public class Mod : BaseMod, IModSystem, IAutoRegisterAll // IAutoRegisterAll does all of the CustomItems, Groups<T> and CustomDishes
    {
        /*
         * GUID: A unique identifier for your Main. This should be unique to your Main. Once you set it, do not change it.
         * NAME: The name of your Main. This is what will be displayed in the mod manager.
         * VERSION: The version of your Main. This is what will be displayed in the mod manager.
         * AUTHOR: Your name.
         * GAMEVERSION: The version of the game that your mod is compatible with. This is uses Semantic Versioning which can be found here: https://semver.org/
         */
        public const string MOD_GUID = "pinemoose.platevsplate";
        public const string MOD_NAME = "Plate Vs Plate";
        public const string MOD_VERSION = "0.3";
        public const string MOD_AUTHOR = "PineMoose";
        public const string MOD_GAMEVERSION = ">=1.3.0";

        internal static Type preferenceSystemMenuType = null;

        /*
         * Logger: This is the logger that you will use to log information to the console.
         */
        internal static KitchenLogger Logger;

        /*
         * Mod Constructor This is where you will set the GUID, NAME, VERSION, AUTHOR, and GAMEVERSION for your Main.
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
            Mod.Logger.LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
           // PlateVsPlate.Team.TeamChecks.PvPModSettings.RegisterPreferences();
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
            Logger = InitLogger();
            Mod.Logger.LogInfo("TestMod loaded successfully!");
            Mod.Logger.LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
            PvPModSettings.RegisterPreferences();

            PauseMenuPreferencesesMenu.RegisterUsableMenu(typeof(PvPSettingsMenu<MenuAction>));
            PauseMenuPreferencesesMenu.RegisterUsableMenu(typeof(PvPColorMenu<MenuAction>));

            Events.MainMenu_SetupEvent += (s, args) =>
            {
                args.addSubmenuButton.Invoke(args.instance, new object[] { "PvP Settings", typeof(PvPSettingsMenu<MenuAction>), false });
            };
        }
    }
}