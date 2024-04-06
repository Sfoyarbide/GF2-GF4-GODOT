using Godot;
using System;
using System.Collections.Generic;

public partial class Slime : CharacterEnemy
{
    public override void _Ready()
    {
        base._Ready();
    }

    protected override void Pattern()
    {
        switch(TurnEnemyPassed)
        {
            case 0:
                CurrentAttack = FindAttack("AttackDOWN");
                break;
            case 1:
                CurrentAttack = FindAttack("AttackUP");
                break;
            case 2:
                CurrentAttack = FindAttack("Fire");
                break;
            case 3:
                CurrentAttack = RandomizeAttack();
                break;
        }
        OnEnemySearchingReceptorList();
        TurnEnemyPassed++;
        
        CheckResetPattern(3);
    }

    protected override void OnFailedSelection(Attack attack)
    {
        CurrentAttack = FindAttack("MeleeAttack");
        OnEnemySearchingReceptorList();
    }
}