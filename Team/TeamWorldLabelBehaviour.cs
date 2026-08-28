using UnityEngine;
using System.Collections.Generic;

namespace TestMod.Team
{
    public class TeamWorldLabelBehaviour : MonoBehaviour
    {
        public static List<(Vector3 worldPos, string text, int team)> Labels = new List<(Vector3, string, int)>();

        private GUIStyle _teamAStyle;
        private GUIStyle _teamBStyle;

        void OnGUI()
        {
            if (_teamAStyle == null)
            {
                _teamAStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 16,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Color.red }
                };
                _teamBStyle = new GUIStyle(_teamAStyle)
                {
                    normal = { textColor = Color.blue }
                };
            }

            if (Camera.main == null) return;

            foreach (var (worldPos, text, team) in Labels)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

                // Behind the camera — skip
                if (screenPos.z < 0) continue;

                // Unity screen space has Y flipped vs GUI space
                float guiY = Screen.height - screenPos.y;

                var style = team == 0 ? _teamAStyle : _teamBStyle;
                GUI.Label(new Rect(screenPos.x - 50, guiY - 20, 100, 30), text, style);
            }
        }
    }
}