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
    }

    private void BaseAction_AttackState(object sender, BaseAction.AttackStateEventArgs e)
    {
        RegisterAttackState(e);
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

    private bool QuerryAttackState<T>(int rewindedTurn, bool isEnemy)
    {
        List<BaseAction.AttackStateEventArgs> attackStateList = GetAttackStateList(isEnemy);
    
        for(int x = attackStateList.Count; x > rewindedTurn; x--)
        {
            if(attackStateList[x].attack is T)
            {   
                return true;
            }
        }

        return false;
    }
}