using Godot;
using System;

public partial class SlimeData : CharacterData
{
    public override void _Ready()
    {
        AttackElementStatusDictionary.Add(AttackTypes.Strike, ElementStatus.Weakness);
    }
}