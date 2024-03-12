using Godot;
using System;

public partial class Test : Node
{
    [Export]
    Character character;
    [Export]
    Character character2;
    public override void _Process(double delta)
    {
        if(Input.IsActionJustPressed("confirm"))
        {
            GD.Print("Is Hit calculation:" + CombatCalculations.IsHitCalculation(character, character2));
            GD.Print("Is Critic Hit calculation:" + CombatCalculations.IsCriticHitCalculation(character, character2));
            //GD.Print(CombatCalculations.DamageCalculation(character, character2, character.DataContainer.MeleeAttack));
        }
    }
}