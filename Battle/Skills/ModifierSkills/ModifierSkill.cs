using Godot;
using System;

public partial class ModifierSkill : Skill
{
    private ModifierStatsInflict _modifierStatsInflict;

    public ModifierStatsInflict ModifierStatsInflict { get {return _modifierStatsInflict;} set {_modifierStatsInflict = value; }}
    public override void UseSkill(Character character)
    {
        InflictStateSkill(character, _modifierStatsInflict);
    }

    private void InflictStateSkill(Character character, ModifierStatsInflict modifierStatsInflict)
    {
        _modifierStatsInflict = modifierStatsInflict;
        
        ModifierStatsInflict opposedModifier = character.DataContainer.ModifierStatsInflictList.Find(x => x.InflictStateType == modifierStatsInflict.OpposedModifierStatsInflictType);
        
        if(opposedModifier != null)
        {
            opposedModifier.EndInflictState(character);
        }
        
        character.DataContainer.ModifierStatsInflictList.Add(modifierStatsInflict);
        modifierStatsInflict.UseInflictEffect(character);
    }
}
