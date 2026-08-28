using HarmonyLib;
using Kitchen;
using System.Collections.Generic;
using TestMod;
using TestMod.Team;
using Unity.Entities;
using UnityEngine;

[HarmonyPatch(typeof(GrantMoneyForSatisfactions), "HandleSatisfiedOrder")]
public static class InterceptSatisfiedOrderMoney
{
    static readonly Dictionary<Entity, int> _amountBeforeCall = new Dictionary<Entity, int>();

    static void Prefix(CItemTransferAccept acceptance, CItemTransferProposal proposal, ref COrderAcceptance details)
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null) return;
        var em = world.EntityManager;

        Entity group = details.Group;
        if (!em.Exists(group)) return;
        if (!em.HasComponent<CGroupReward>(group)) return;

        int before = em.GetComponentData<CGroupReward>(group).Amount;
        _amountBeforeCall[group] = before;
    }

    static void Postfix(CItemTransferAccept acceptance, CItemTransferProposal proposal, ref COrderAcceptance details)
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null) return;
        var em = world.EntityManager;

        Entity group = details.Group;
        Entity table = details.Source;

        if (!em.Exists(table) || !em.HasComponent<CTeamAssignment>(table))
        {
            _amountBeforeCall.Remove(group);
            return;
        }
        if (!em.Exists(group) || !em.HasComponent<CGroupReward>(group))
        {
            _amountBeforeCall.Remove(group);
            return;
        }

        int after = em.GetComponentData<CGroupReward>(group).Amount;
        if (!_amountBeforeCall.TryGetValue(group, out int before))
        {
            return;
        }

        int delta = after - before;
        var teamAssignment = em.GetComponentData<CTeamAssignment>(table);
        int team = teamAssignment.Team;

        var teamData = TeamMoney.Get(team);
        teamData.Balance += delta;

        int dishIdentifier = details.CreditDish;
        if (dishIdentifier != 0)
        {
            teamData.RecordDish(dishIdentifier);
            Mod.Logger.LogInfo($"[TestMod] Team {team} served dish {dishIdentifier} for ${delta}");
        }

        _amountBeforeCall.Remove(group);
    }
}