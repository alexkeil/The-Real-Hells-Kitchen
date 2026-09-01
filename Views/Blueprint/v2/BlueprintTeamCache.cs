namespace PlateVsPlate.Views.v2
{
    /*
    public class BlueprintTeamCache : UpdatableObjectView<BlueprintTeamCache.ViewData>
    {
        public int CachedTeam = -1;
        public int CachedPrice = -1;

        protected override void UpdateData(ViewData data)
        {
            CachedTeam = data.Team;
            CachedPrice = data.Price;
        }

        public class UpdateView : IncrementalViewSystemBase<ViewData>, IModSystem
        {
            private EntityQuery query;

            protected override void Initialise()
            {
                query = GetEntityQuery(new QueryHelper().All(typeof(CTeamMarker), typeof(CForSale), typeof(CLinkedView)));
            }

            protected override void OnUpdate()
            {
                var entities = query.ToEntityArray(Allocator.Temp);
                foreach (var entity in entities)
                {
                    Require<CLinkedView>(entity, out var view);
                    Require<CTeamMarker>(entity, out var marker);
                    Require<CForSale>(entity, out var sale);

                    SendUpdate(view, new ViewData { Team = marker.Team, Price = sale.Price }, MessageType.SpecificViewUpdate);
                }
                entities.Dispose();
            }
        }

        [MessagePackObject(false)]
        public struct ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)] public int Team;
            [Key(1)] public int Price;
            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<BlueprintTeamCache>();
            public bool IsChangedFrom(ViewData check) => Team != check.Team || Price != check.Price;
        }
    }
    */
}