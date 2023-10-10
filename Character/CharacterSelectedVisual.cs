using Godot;
using System;

public partial class CharacterSelectedVisual : Sprite3D
{
    private Character _character;
    private bool isShowing = false;
    public override void _Ready()
    {
        _character = GetParent<Character>();        
        CharacterReceptorSelector.OnCharacterReceptorSelected += CharacterReceptorSelector_OnCharacterReceptorSelected;
        Hide();
    }

    private void CharacterReceptorSelector_OnCharacterReceptorSelected(object sender, CharacterReceptorSelector.OnCharacterReceptorSelectedEventArgs e)
    {
        UpdateSelectedVisual(e.characterRecepetor);
    }

    private void UpdateSelectedVisual(Character characterReceptor)
    {
        if(characterReceptor == _character)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }   
}