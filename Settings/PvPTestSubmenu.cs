using HarmonyLib;
using Kitchen;
using Kitchen.Modules;
using KitchenLib;
using UnityEngine;

namespace PlateVsPlate.Setttings
{
    public class PvPTestSubmenu<T> : KLMenu<T>
    {
        private MainMenu _parentMainMenu;
        private int _returnPlayerId;

        public PvPTestSubmenu(Transform container, ModuleList module_list, MainMenu parentMainMenu, int returnPlayerId) : base(container, module_list)
        {
            _parentMainMenu = parentMainMenu;
            _returnPlayerId = returnPlayerId;
        }

        public override void Setup(int player_id)
        {
            AddLabel("This is a test submenu");

            New<SpacerElement>();
            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate
            {
                var container = (Transform)AccessTools.Field(typeof(Menu<T>), "Container").GetValue(this);
                var moduleList = (ModuleList)AccessTools.Field(typeof(Menu<T>), "ModuleList").GetValue(this);
                moduleList.Clear();

                var settingsMenu = new PvPSettingsMenu<T>(container, moduleList);
                settingsMenu.Setup(player_id);
            });
        }
    }
}