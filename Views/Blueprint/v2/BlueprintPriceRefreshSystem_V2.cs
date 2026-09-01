namespace PlateVsPlate.Views.v2
{
    /*
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class BlueprintPriceRefreshSystem_V2 : GenericSystemBase, IModSystem
    {
        private const string AffordableTag = "<sprite name=\"coin\" color=#FF9800>";
        private const string UnaffordableTag = "<sprite name=\"coin\" color=#660700>";

        protected override void OnUpdate()
        {
            var views = Object.FindObjectsOfType<BlueprintView>();
            foreach (var view in views)
            {
                var cache = view.GetComponent<BlueprintTeamCache>();
                if (cache == null)
                {
                    cache = view.gameObject.AddComponent<BlueprintTeamCache>();
                }

                if (cache.CachedTeam < 0 || cache.CachedPrice < 0) continue;

                int team = cache.CachedTeam;
                bool canAfford = TeamData.Get(team).Balance >= cache.CachedPrice;

                var type = typeof(BlueprintView);
                var priceIconField = type.GetField("PriceIcon", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                var titleField = type.GetField("Title", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                var priceIcon = priceIconField?.GetValue(view) as TMPro.TextMeshPro;
                var titleText = titleField?.GetValue(view) as TMPro.TextMeshPro;

                if (titleText != null) titleText.color = PvPTeamColors.GetTeamColor(team);
                if (priceIcon == null) continue;

                string current = priceIcon.text;
                string wantedTag = canAfford ? AffordableTag : UnaffordableTag;
                string otherTag = canAfford ? UnaffordableTag : AffordableTag;
                if (current.EndsWith(otherTag))
                    priceIcon.text = current.Substring(0, current.Length - otherTag.Length) + wantedTag;
            }
        }
    }
    */
}