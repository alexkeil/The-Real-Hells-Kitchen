using HarmonyLib;
using Kitchen;
using UnityEngine;

namespace PlateVsPlate.Views.TeamMoney
{
    [HarmonyPatch(typeof(MoneyDisplayView), "UpdateData", new System.Type[] { typeof(MoneyDisplayView.ViewData) })]
    internal static class MoneyDisplayUpdate_Patch
    {
        private static GameObject _team0MoneyPanel;
        private static GameObject _team0StrikesPanel;
        private static GameObject _team1MoneyPanel;
        private static GameObject _team1StrikesPanel;

        private static TMPro.TextMeshPro _team0MoneyText;
        private static TMPro.TextMeshPro _team0StrikesText;
        private static TMPro.TextMeshPro _team1MoneyText;
        private static TMPro.TextMeshPro _team1StrikesText;

        public static bool PanelsReady => _team0MoneyPanel != null;

        public static void SetTeam0Money(string text) { if (_team0MoneyText != null) _team0MoneyText.text = text; }
        public static void SetTeam0Strikes(string text) { if (_team0StrikesText != null) _team0StrikesText.text = text; }
        public static void SetTeam1Money(string text) { if (_team1MoneyText != null) _team1MoneyText.text = text; }
        public static void SetTeam1Strikes(string text) { if (_team1StrikesText != null) _team1StrikesText.text = text; }

        [HarmonyPostfix]
        private static void Postfix(MoneyDisplayView __instance, MoneyDisplayView.ViewData view_data)
        {
            if (_team0MoneyPanel == null)
            {
                BuildPanels(__instance);
            }
        }

        public static void SetVisible(bool visible)
        {
            if (_team0MoneyPanel != null) _team0MoneyPanel.SetActive(visible);
            if (_team0StrikesPanel != null) _team0StrikesPanel.SetActive(visible);
            if (_team1MoneyPanel != null) _team1MoneyPanel.SetActive(visible);
            if (_team1StrikesPanel != null) _team1StrikesPanel.SetActive(visible);
        }

        private static void BuildPanels(MoneyDisplayView original)
        {
            Color team0Color = new Color(0.9f, 0.2f, 0.2f, 1f);
            Color team1Color = new Color(0.2f, 0.5f, 0.9f, 1f);

            // Team 0
            _team0MoneyPanel = MakePanel(original, new Vector3(2f, 0f, 0f), team0Color, hideCoin: false);
            _team0MoneyText = FindChildText(_team0MoneyPanel, "Value");

            _team0StrikesPanel = MakePanel(original, new Vector3(2f, -0.5f, 0f), team0Color, hideCoin: true);
            _team0StrikesText = FindChildText(_team0StrikesPanel, "Value");

            // Team 1
            _team1MoneyPanel = MakePanel(original, new Vector3(2f, -1.5f, 0f), team1Color, hideCoin: false);
            _team1MoneyText = FindChildText(_team1MoneyPanel, "Value");

            _team1StrikesPanel = MakePanel(original, new Vector3(2f, -2f, 0f), team1Color, hideCoin: true);
            _team1StrikesText = FindChildText(_team1StrikesPanel, "Value");

        }

        private static GameObject MakePanel(MoneyDisplayView original, Vector3 offset, Color color, bool hideCoin)
        {
            var panel = Object.Instantiate(original.gameObject, original.transform.parent);
            panel.transform.localPosition = original.transform.localPosition + offset;
            Object.Destroy(panel.GetComponent<MoneyDisplayView>());
            if (hideCoin) HideChild(panel, "Unit");
            SetQuadHighlightColor(panel, color);
            return panel;
        }

        private static TMPro.TextMeshPro FindChildText(GameObject parent, string childName)
        {
            var allTexts = parent.GetComponentsInChildren<TMPro.TextMeshPro>(true);
            foreach (var t in allTexts) if (t.gameObject.name == childName) return t;
            return null;
        }

        private static void HideChild(GameObject parent, string childName)
        {
            var allTransforms = parent.GetComponentsInChildren<Transform>(true);
            foreach (var t in allTransforms)
            {
                if (t.gameObject.name == childName) { t.gameObject.SetActive(false); return; }
            }
        }

        public static void ResetPanels()
        {
            if (_team0MoneyPanel != null) Object.Destroy(_team0MoneyPanel);
            if (_team0StrikesPanel != null) Object.Destroy(_team0StrikesPanel);
            if (_team1MoneyPanel != null) Object.Destroy(_team1MoneyPanel);
            if (_team1StrikesPanel != null) Object.Destroy(_team1StrikesPanel);

            _team0MoneyPanel = null;
            _team0StrikesPanel = null;
            _team1MoneyPanel = null;
            _team1StrikesPanel = null;
            _team0MoneyText = null;
            _team0StrikesText = null;
            _team1MoneyText = null;
            _team1StrikesText = null;
        }

        private static void SetQuadHighlightColor(GameObject parent, Color color)
        {
            var allTransforms = parent.GetComponentsInChildren<Transform>(true);
            foreach (var t in allTransforms)
            {
                if (t.gameObject.name == "Quad")
                {
                    var mat = t.GetComponent<MeshRenderer>()?.material;
                    if (mat != null && mat.HasProperty("_Highlight")) mat.SetColor("_Highlight", color);
                    return;
                }
            }
        }
    }
}