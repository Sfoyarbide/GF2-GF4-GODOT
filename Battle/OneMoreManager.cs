using Godot;
using System;
using System.Collections.Generic;

public partial class OneMoreManager : Node
{
    private List<bool> _isOneMore = new List<bool>();

    public override void _Ready()
    {
        BaseAction.AttackState += BaseAction_AttackState;
        BattleManager.OnTurnEnd += BattleManager_OnTurnEnd;
    }

    private void BaseAction_AttackState(object sender, BaseAction.AttackStateEventArgs e)
    {
        _isOneMore.Clear();

        bool isOneMore = IsOneMore(e);

        if(isOneMore == true || !e.receptor.DataContainer.IsDefending)
        {
            e.receptor.DataContainer.AlreadyHitWeakness = true;
        }

        _isOneMore.Add(isOneMore);
    }

    private void BattleManager_OnTurnEnd(object sender, EventArgs e)
    {
        _isOneMore.Clear();
    }

    // Checks based on the game rules, if you have one more turn.
    private bool IsOneMore(BaseAction.AttackStateEventArgs attackState)
    {
        if(attackState.isHit == false)
        {
            return false;
        }

        if(attackState.receptor.DataContainer.AlreadyHitWeakness)
        {
            return false;
        }

        if(attackState.baseAction is ItemAction)
        {
            return false;
        }

        if(attackState.receptor.DataContainer.InflictState is KnockDown)
        {
            return false;
        }

        if(attackState.isPressionAttack == true)
        {
            return false;
        }

        return attackState.receptor.DataContainer.IsElementStatusToAttackType(attackState.attack.AttackType, ElementStatus.Weakness);
    }

    public bool HaveOneMore()
    {
        if(_isOneMore.Count == 1)
        {
            return _isOneMore[0];
        }

        bool haveOneMore = false;
        foreach(bool oneMore in _isOneMore)
        {
            if(oneMore == true)
            {
                haveOneMore = oneMore;
            }
        }
        return haveOneMore;
    }
}