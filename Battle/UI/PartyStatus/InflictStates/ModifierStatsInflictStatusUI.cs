using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class ModifierStatsInflictStatusUI : Panel
{
    private Character _character;
    private TextureRect _modifierStatusUI;

    public enum ModifierStatsInflictStatusUIType
    {
        At,
        De,
        Ag
    }

    [Export] // Assign by Inspector.
    private ModifierStatsInflictStatusUIType type;

    public override void _Ready()
    {
        _modifierStatusUI = GetNode<TextureRect>("ModifierStatusUI");
        ModifierStatsInflict.ModifierStatsInflicted += ModifierStatsInflict_ModifierStatsInflicted;
        ModifierStatsInflict.ModifierStatsEnded += ModifierStatsInflict_ModifierStatsEnded;
    }

    public void Setup(Character character)
    {
        _character = character;
    }

    private void ModifierStatsInflict_ModifierStatsInflicted(object sender, ModifierStatsInflict.ModifierStatsEventArgs e)
    {
        if(e.characterWithEffect != _character)
        {
            return;
        }

        switch(type)
        {
            case ModifierStatsInflictStatusUIType.At:
                if(e.modifierStatsInflict.InflictStateType == InflictStates.AttackUP)
                {
                    Show();
                    _modifierStatusUI.FlipV = false;
                } 
                if(e.modifierStatsInflict.InflictStateType == InflictStates.AttackDOWN)
                {
                    Show();
                    _modifierStatusUI.FlipV = true;
                }
                break;
            case ModifierStatsInflictStatusUIType.De:
                if(e.modifierStatsInflict.InflictStateType == InflictStates.DefendUP)
                {
                    Show();
                    _modifierStatusUI.FlipV = false;
                } 
                if(e.modifierStatsInflict.InflictStateType == InflictStates.DefendDOWN)
                {
                    Show();
                    _modifierStatusUI.FlipV = true;
                }
                break;
            case ModifierStatsInflictStatusUIType.Ag:
                if(e.modifierStatsInflict.InflictStateType == InflictStates.AgilityUP)
                {
                    Show();
                    _modifierStatusUI.FlipV = false;
                } 
                if(e.modifierStatsInflict.InflictStateType == InflictStates.AgilityDOWN)
                {
                    Show();
                    _modifierStatusUI.FlipV = true;
                }
                break;
        }
    }

    private void ModifierStatsInflict_ModifierStatsEnded(object sender, ModifierStatsInflict.ModifierStatsEventArgs e)
    {
        switch(type)
        {
            case ModifierStatsInflictStatusUIType.At:
                if(e.modifierStatsInflict.InflictStateType == InflictStates.AttackUP || e.modifierStatsInflict.InflictStateType == InflictStates.AttackDOWN)
                {
                    Hide();
                } 
                break;
            case ModifierStatsInflictStatusUIType.De:
                if(e.modifierStatsInflict.InflictStateType == InflictStates.DefendUP || e.modifierStatsInflict.InflictStateType == InflictStates.DefendDOWN)
                {
                    Hide();
                } 
                break;
            case ModifierStatsInflictStatusUIType.Ag:
                if(e.modifierStatsInflict.InflictStateType == InflictStates.AgilityUP || e.modifierStatsInflict.InflictStateType == InflictStates.AgilityDOWN)
                {
                    Hide();
                }
                break;
        }
    }
}