using Kitchen;
using KitchenMods;
using MessagePack;
using PlateVsPlate.Settings;
using PlateVsPlate.Team;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;


namespace PlateVsPlate.Views.Appliance
{
    public class ApplianceTintSubview : UpdatableObjectView<ApplianceTintSubview.ViewData>
    {
        protected override void UpdateData(ViewData data)
        {
            Color tint = PvPTeamColors.GetTeamColor(data.Team);
            foreach (var renderer in GetComponentsInChildren<MeshRenderer>(true))
            {
                var mat = renderer.material;
                if (mat.HasProperty("_OverlayColour")) mat.SetColor("_OverlayColour", tint);
                if (mat.HasProperty("_HasTextureOverlay")) mat.SetFloat("_HasTextureOverlay", 1f);
                if (mat.HasProperty("_Color0")) mat.SetColor("_Color0", tint);
                if (mat.HasProperty("_Colour2")) mat.SetColor("_Colour2", tint);
                if (mat.HasProperty("_Color2")) mat.SetColor("_Color2", tint);
                if (mat.HasProperty("_Color")) mat.SetColor("_Color", tint);
                if (mat.HasProperty("_Highlight")) mat.SetFloat("_Highlight", 0.3f);
            }
        }

        public class UpdateView : IncrementalViewSystemBase<ViewData>, IModSystem
        {
            private EntityQuery query;

            protected override void Initialise()
            {
                query = GetEntityQuery(
                    new QueryHelper()
                    .All(typeof(CTeamMarker), typeof(CLinkedView))
                    .None(typeof(CForSale)));
            }

            protected override void OnUpdate()
            {
                var entities = query.ToEntityArray(Allocator.Temp);
                foreach (var entity in entities)
                {
                    Require<CLinkedView>(entity, out var view);
                    Require<CTeamMarker>(entity, out var marker);
                    SendUpdate(view, new ViewData { Team = marker.Team }, MessageType.SpecificViewUpdate);
                }
                entities.Dispose();
            }
        }

        [MessagePackObject(false)]
        public struct ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)] public int Team;
            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<ApplianceTintSubview>();
            public bool IsChangedFrom(ViewData check) => Team != check.Team;
        }
    }
}