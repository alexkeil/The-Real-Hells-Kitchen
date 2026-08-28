using UnityEngine;

namespace TestMod.Team
{
    public class TeamDebugOverlayBehaviour : MonoBehaviour
    {
        public static string MoneyText = "Mo Money, Mo Problems";

        void OnGUI()
        {
            GUI.Label(new Rect(10, 200, 600, 300), $"[Team Money] {MoneyText}");
        }
    }
}