using Godot;
using System;
using System.Collections.Generic;

public partial class BattleRegister : Node
{
    private List<BaseAction.AttackStateEventArgs> _attackEnemiesStatesList = new List<BaseAction.AttackStateEventArgs>();
    private List<BaseAction.AttackStateEventArgs> _attackPartyStatesList = new List<BaseAction.AttackStateEventArgs>();

    public override void _Ready()
    {
        // Subcribing to events.
        BaseAction.AttackState += BaseAction_AttackState;
        BattleManager.OnBattleEnd += BattleManager_OnBattleEnd;
    }
    private void RegisterAttackState(BaseAction.AttackStateEventArgs attackState)
    {
        GetAttackStateList(attackState.current.DataContainer.IsEnemy).Add(attackState);
    }

    private List<BaseAction.AttackStateEventArgs> GetAttackStateList(bool isEnemy)
    {
        if(isEnemy)
        {
            return _attackEnemiesStatesList;
        }
        else
        {
            return _attackPartyStatesList;
        }
    }

    public bool QuerryAttackState<T>(int rewindedTurn, bool isEnemy) where T : Attack
    {
        List<BaseAction.AttackStateEventArgs> attackStateList = GetAttackStateList(isEnemy);

        int condicionToRewinded = 0;

        if(attackStateList.Count - 1 < rewindedTurn)
        {
            condicionToRewinded = 0;
        }
        else
        {
            condicionToRewinded = attackStateList.Count - rewindedTurn - 1;
        }

        for(int x = attackStateList.Count - 1; x >= condicionToRewinded; x--)
        {
            if(attackStateList[x].attack is T)
            {   
                GD.Print(attackStateList[x].attack.AttackName + "is: t");
                return true;
            }
        }

        return false;
    }

    private void BaseAction_AttackState(object sender, BaseAction.AttackStateEventArgs e)
    {
        RegisterAttackState(e);
    }

    private void BattleManager_OnBattleEnd(object sender, BattleManager.OnBattleEndEventArgs e)
    {
        _attackEnemiesStatesList.Clear();
        _attackPartyStatesList.Clear();
    }
}