using Godot;
using System;

public partial class InflictSkill : Skill
{
    private InflictState _inflictState;

    public InflictState InflictState { get { return _inflictState; } set {_inflictState = value;}}

    public override void UseSkill(Character character, Character receptor, out int damage)
    {
        damage = 0;
        _inflictState.InflictCharacter(receptor);
    }

}
