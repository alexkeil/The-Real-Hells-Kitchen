using Kitchen.Modules;
using Kitchen;
using KitchenLib;
using System.Collections.Generic;
using PlateVsPlate.Team.TeamChecks;
using UnityEngine;

namespace PlateVsPlate.Setttings
{
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
}