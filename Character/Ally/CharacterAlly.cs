using Godot;
using System;

public partial class CharacterAlly : CharacterData
{
    public override void _Ready()
    {
        base._Ready();
        IndividualPressionInflictStateType = InflictStates.KnockDown;
    }
}
