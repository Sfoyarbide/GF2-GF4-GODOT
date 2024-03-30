using Godot;
using System;

public partial class SlimeData : CharacterData
{
    public override void _Ready()
    {
        base._Ready();
        CharacterName = "Slime";
        AttackElementStatusDictionary.Add(AttackTypes.Strike, ElementStatus.Weakness);
    }
}