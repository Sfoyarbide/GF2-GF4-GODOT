using Godot;
using System;
using System.Collections.Generic;

public partial class Test : Node
{
    public List<Character> allyList = new List<Character>();
    public List<Character> enemyList = new List<Character>();

    public override void _Ready()
    {
        for(int x = 0; x < GetChildCount(); x++)
        {
            Character character = (Character)GetChild(x);
            if(character.DataContainer.IsEnemy)
            {
                enemyList.Add(character);
            }
            else
            {
                allyList.Add(character);
            }
        }
    }

    public override void _Process(double delta)
    {
        if(Input.IsActionJustPressed("confirm"))
        {
            GD.Print(CombatCalculations.AllOutAttackDamageCalculation(allyList, enemyList));
        }
    }
}