using Kitchen;
using KitchenMods;
using MessagePack;
using PlateVsPlate.Settings;
using PlateVsPlate.Team;
using Unity.Collections;
using Unity.Entities;

namespace PlateVsPlate.Views.TeamMoney
{
    public class TeamMoneyNetworkedSubview : UpdatableObjectView<TeamMoneyNetworkedSubview.ViewData>
    {
        protected override void UpdateData(ViewData data)
        {
            if (data.Team == 0)
            {
                MoneyDisplayUpdate_Patch.SetTeam0Money($"{data.Balance}");
                MoneyDisplayUpdate_Patch.SetTeam0Strikes(StrikesText(data.Strikes, data.MaxStrikes));
            }
            else if (data.Team == 1)
            {
                MoneyDisplayUpdate_Patch.SetTeam1Money($"{data.Balance}");
                MoneyDisplayUpdate_Patch.SetTeam1Strikes(StrikesText(data.Strikes, data.MaxStrikes));
            }
        }

        private static string StrikesText(int strikes, int max) =>
            new string('X', strikes) + new string('_', UnityEngine.Mathf.Max(0, max - strikes));

        public class UpdateView : IncrementalViewSystemBase<ViewData>, IModSystem
        {
            private EntityQuery query;

            protected override void Initialise()
            {
                query = GetEntityQuery(new QueryHelper().All(typeof(CTeamMarker), typeof(CLinkedView)));
            }

            protected override void OnUpdate()
            {
                var entities = query.ToEntityArray(Allocator.Temp);
                foreach (var entity in entities)
                {
                    Require<CLinkedView>(entity, out var view);
                    Require<CTeamMarker>(entity, out var marker);

                    var teamData = TeamData.Get(marker.Team);
                    SendUpdate(view, new ViewData
                    {
                        Team = marker.Team,
                        Balance = teamData.Balance,
                        Strikes = teamData.Strikes,
                        MaxStrikes = PvPStrikeSettings.StrikesBeforeElimination
                    }, MessageType.SpecificViewUpdate);
                }
                entities.Dispose();
            }
        }

        [MessagePackObject(false)]
        public struct ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)] public int Team;
            [Key(1)] public int Balance;
            [Key(2)] public int Strikes;
            [Key(3)] public int MaxStrikes;

            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<TeamMoneyNetworkedSubview>();
            public bool IsChangedFrom(ViewData check) =>
                Team != check.Team || Balance != check.Balance || Strikes != check.Strikes || MaxStrikes != check.MaxStrikes;
        }
    }
}