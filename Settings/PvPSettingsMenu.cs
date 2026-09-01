using UnityEngine;
using Kitchen.Modules;
using Kitchen;
using System.Collections.Generic;
using KitchenLib;

namespace PlateVsPlate.Setttings
{
    public class PvPSettingsMenu<T> : KLMenu<T>
    {
        private static readonly List<bool> boolValues = new List<bool> { false, true };
        private static readonly List<string> boolLabels = new List<string> { "Off", "On" };

        private static readonly List<int> strikeValues = new List<int> { 1, 2, 3, 4, 5 };
        private static readonly List<string> strikeLabels = new List<string> { "1", "2", "3", "4", "5" };

        public PvPSettingsMenu(Transform container, ModuleList module_list) : base(container, module_list) { }

        public override void Setup(int player_id)
        {
            addShowPopupSelect();
            addStrikesSelect();
            addBalanceModeSelect();

            New<SpacerElement>();
            AddSubmenuButton("Colors", typeof(PvPColorMenu<T>), false);

            addBackButton();
        }

        private void addShowPopupSelect()
        {
            Option<bool> option = new Option<bool>(boolValues, PvPModSettings.GetShowRestartPopup(), boolLabels);

            AddLabel("Show Restart Popup");
            AddInfo("If On, players confirm each restart via a popup. If Off, restarts happen immediately.");
            AddSelect(option);

            option.OnChanged += delegate (object _, bool value)
            {
                PvPModSettings.SetShowRestartPopup(value);
            };
        }

        private void addStrikesSelect()
        {
            Option<int> option = new Option<int>(strikeValues, PvPModSettings.GetStrikesBeforeElimination(), strikeLabels);

            AddLabel("Strikes Before Elimination");
            AddInfo("How many table failures a team can have before elimination.");
            AddSelect(option);

            option.OnChanged += delegate (object _, int value)
            {
                PvPModSettings.SetStrikesBeforeElimination(value);
            };
        }

        private void addBalanceModeSelect()
        {
            Option<bool> option = new Option<bool>(boolValues, PvPModSettings.GetBalanceByCustomerCount(), new List<string> { "By Group Count", "By Customer Count" });

            AddLabel("Balance Mode");
            AddInfo("How to balance incoming customers between teams.");
            AddSelect(option);

            option.OnChanged += delegate (object _, bool value)
            {
                PvPModSettings.SetBalanceByCustomerCount(value);
            };
        }

        private void addBackButton()
        {
            New<SpacerElement>();
            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate { RequestPreviousMenu(); });
        }
    }

    /*
    public class PvPColorMenu<T> : KLMenu<T>
    {
        private static readonly List<int> colorValues = new List<int> { 0, 1, 2, 3 };
        private static readonly List<string> colorLabels = new List<string> { "Red", "Blue", "Green", "Yellow" };

        public PvPColorMenu(Transform container, ModuleList module_list) : base(container, module_list) { }

        public override void Setup(int player_id)
        {
            AddLabel("Team A");
            Option<int> teamAOption = new Option<int>(colorValues, PvPModSettings.GetTeam0ColorIndex(), colorLabels);
            AddSelect(teamAOption);
            teamAOption.OnChanged += delegate (object _, int value)
            {
                PvPModSettings.SetTeam0ColorIndex(value);
            };

            New<SpacerElement>();

            AddLabel("Team B");
            Option<int> teamBOption = new Option<int>(colorValues, PvPModSettings.GetTeam1ColorIndex(), colorLabels);
            AddSelect(teamBOption);
            teamBOption.OnChanged += delegate (object _, int value)
            {
                PvPModSettings.SetTeam1ColorIndex(value);
            };

            New<SpacerElement>();
            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate { RequestPreviousMenu(); });
        }
    }
    */
}