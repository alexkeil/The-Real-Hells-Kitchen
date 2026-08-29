using Kitchen;
using KitchenMods;
using TestMod.Team;
using UnityEngine;

namespace TestMod.Team
{
    public class CreateTeamMoneyDisplays : GenericSystemBase, IModSystem
    {
        private bool _created = false;
        private TMPro.TextMeshPro _team0Text;
        private TMPro.TextMeshPro _team1Text;

        protected override void OnUpdate()
        {
            if (!_created)
            {
                var original = Object.FindObjectOfType<MoneyDisplayView>();
                if (original == null) return;

                var copy0 = Object.Instantiate(original.gameObject, original.transform.parent);
                var copy1 = Object.Instantiate(original.gameObject, original.transform.parent);

                copy0.transform.localPosition = original.transform.localPosition + new Vector3(2f, -0.5f, 0f);
                copy1.transform.localPosition = original.transform.localPosition + new Vector3(2f, -1.2f, 0f);

                Object.Destroy(copy0.GetComponent<MoneyDisplayView>());
                Object.Destroy(copy1.GetComponent<MoneyDisplayView>());

                _team0Text = copy0.GetComponentInChildren<TMPro.TextMeshPro>();
                _team1Text = copy1.GetComponentInChildren<TMPro.TextMeshPro>();

                original.gameObject.SetActive(false);

                _created = true;
                Mod.Logger.LogInfo($"[DEBUGGING] Created displays. team0Text null: {_team0Text == null}, team1Text null: {_team1Text == null}");
            }

            if (_team0Text != null) _team0Text.text = $"Team A: {TeamMoney.Get(0).Balance}";
            if (_team1Text != null) _team1Text.text = $"Team B: {TeamMoney.Get(1).Balance}";
        }
    }
}