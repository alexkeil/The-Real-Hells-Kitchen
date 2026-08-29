using Kitchen;
using KitchenMods;
using System.Collections.Generic;
using Unity.Entities;

namespace TestMod.Team.TeamChecks
{
    public static class PendingPurchases
    {
        public static readonly Dictionary<int, (int team, int price)> Entries = new Dictionary<int, (int, int)>();
    }

    public class CheckCompletedPurchases : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            if (PendingPurchases.Entries.Count == 0) return;

            var query = GetEntityQuery(ComponentType.ReadOnly<CForSale>());
            var stillForSale = new HashSet<int>();
            var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);
            foreach (var e in entities) stillForSale.Add(e.Index);
            entities.Dispose();

            List<int> toRemove = new List<int>();
            foreach (var kvp in PendingPurchases.Entries)
            {
                if (!stillForSale.Contains(kvp.Key))
                {
                    int team = kvp.Value.team;
                    int price = kvp.Value.price;
                    var teamData = TeamMoney.Get(team);
                    teamData.Balance -= price;
                    Mod.Logger.LogInfo($"[DEBUGGING] Team {team} spent {price} on timed purchase, new balance {teamData.Balance}");
                    toRemove.Add(kvp.Key);
                }
            }
            foreach (var key in toRemove) PendingPurchases.Entries.Remove(key);
        }
    }
}