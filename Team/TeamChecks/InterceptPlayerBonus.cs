using HarmonyLib;
using Kitchen;
using Unity.Entities;
using UnityEngine;

namespace PlateVsPlate.Team.TeamChecks
{
    [HarmonyPatch(typeof(CreateEndOfDayPopup), "OnUpdate")]
    public static class InterceptPlayerBonus
    {
        private static int _processedDay = -1;

        static void Prefix(CreateEndOfDayPopup __instance)
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return;
            var em = world.EntityManager;

            var restartQuery = em.CreateEntityQuery(ComponentType.ReadOnly<SIsRestartedDay>());
            if (!restartQuery.IsEmptyIgnoreFilter) return;

            var dayQuery = em.CreateEntityQuery(ComponentType.ReadOnly<SDay>());
            if (dayQuery.IsEmptyIgnoreFilter) return;
            int day = dayQuery.GetSingleton<SDay>().Day;
            if (day == 0) return;
            if (_processedDay == day) return;

            var moneyQuery = em.CreateEntityQuery(ComponentType.ReadOnly<SMoney>());
            var trackerQuery = em.CreateEntityQuery(ComponentType.ReadOnly<SMoneyEarningsTracker>());
            if (moneyQuery.IsEmptyIgnoreFilter || trackerQuery.IsEmptyIgnoreFilter) return;

            SMoney money = moneyQuery.GetSingleton<SMoney>();
            int oldAmount = trackerQuery.GetSingleton<SMoneyEarningsTracker>().OldAmount;
            int num = money - oldAmount;

            var playerQuery = em.CreateEntityQuery(ComponentType.ReadOnly<CPlayer>());
            int playerCount = playerQuery.CalculateEntityCount();
            float num2 = DifficultyHelpers.MoneyRewardPlayerModifier(playerCount);
            int playerBonus = Mathf.CeilToInt((float)num * (num2 - 1f));

            if (playerBonus == 0 || TeamData.Teams.Count == 0)
            {
                _processedDay = day;
                return;
            }

            // Split evenly across teams 
            int teamCount = TeamData.Teams.Count;
            int share = playerBonus / teamCount;
            int remainder = playerBonus - (share * teamCount);

            int index = 0;
            foreach (var teamData in TeamData.Teams.Values)
            {
                int amount = share + (index == teamCount - 1 ? remainder : 0);
                teamData.EarnMoney(amount);
                index++;
            }

            _processedDay = day;
        }

        public static void Reset()
        {
            _processedDay = -1;
            TeamData.ClearAll();
        }
    }
}