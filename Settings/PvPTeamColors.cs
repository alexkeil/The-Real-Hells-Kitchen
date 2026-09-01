using PlateVsPlate.Setttings;
using UnityEngine;

namespace PlateVsPlate.Settings
{

    public static class PvPTeamColors
    {

        private static readonly Color[] Palette = new Color[]
        {
            new Color(0.9f, 0.2f, 0.2f, 1f), // Red
            new Color(0.2f, 0.5f, 0.9f, 1f), // Blue
            new Color(0.3f, 0.8f, 0.3f, 1f), // Green
            new Color(0.95f, 0.8f, 0.15f, 1f) // Yellow
        };

        public static Color GetColor(int colorIndex)
        {
            if (colorIndex < 0 || colorIndex >= Palette.Length) return Color.white;
            return Palette[colorIndex];
        }

        public static Color GetTeamColor(int team)
        {
            int index = team == 0
                ? PvPModSettings.GetTeam0ColorIndex()
                : PvPModSettings.GetTeam1ColorIndex();
            return GetColor(index);
        }
    }
}