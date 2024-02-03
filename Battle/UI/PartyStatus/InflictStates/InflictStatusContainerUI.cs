using Godot;
using System;
using System.Collections.Generic;

public partial class InflictStatusContainerUI : Panel
{
    private Character _character;
    private Dictionary<string, TextureRect> _inflictStatesDictionary = new Dictionary<string, TextureRect>();

    public override void _Ready()
    {
        InflictState.InflictedCharacter += InflictState_InflictedCharacter;
        InflictState.EndInflictedCharacter += InflictState_EndInflictedCharacter;
        for(int x = 0; x < GetChildCount(); x++)
        {
            TextureRect state = (TextureRect)GetChild(x);
            _inflictStatesDictionary.Add(state.Name, state);
        }
    }

    public void Setup(Character character)
    {
        _character = character;
    }

    private void InflictState_InflictedCharacter(object sender, InflictState.InflictStateEventArgs e)
    {
        if(e.characterWithEffect != _character)
        {
            return;
        }

        foreach(TextureRect states in _inflictStatesDictionary.Values)
        {
            states.Hide();
        }

        Show();
        _inflictStatesDictionary[e.inflict.InflictStateType.ToString()].Show();
    }

    private void InflictState_EndInflictedCharacter(object sender, InflictState.InflictStateEventArgs e)
    {
        if(e.characterWithEffect != _character)
        {
            return;
        }

        Hide();
        _inflictStatesDictionary[e.inflict.InflictStateType.ToString()].Hide();
    }

}