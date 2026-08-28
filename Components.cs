using KitchenMods;
using Unity.Entities;

namespace TestMod
{
    // marks items that have already been duplicated
    public struct StartingAlreadyDuplicatedForTeams : IComponentData, IModComponent { }

    // Marks which team an entity belongs to (0 = left/Team A, 1 = right/Team B)
    public struct CTeamAssignment : IComponentData, IModComponent
    {
        public int Team;
    }

}
