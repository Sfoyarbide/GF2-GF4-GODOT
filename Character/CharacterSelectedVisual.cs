using Godot;
using System;

public partial class CharacterSelectedVisual : Sprite3D
{
    private Character _character;
    private bool isShowing = false;
    public override void _Ready()
    {
        _character = GetParent<Character>();     
        CharacterReceptorSelector.OnCharacterSelectorStarted += CharacterReceptorSelector_OnCharacterSelectorStarted;
        CharacterReceptorSelector.OnCharacterSelectorCanceled += CharacterReceptorSelector_OnCharacterSelectorCanceled;
        CharacterReceptorSelector.OnCharacterSelectorCompleted += CharacterReceptorSelector_OnCharacterSelectorCompleted;
        CharacterReceptorSelector.OnCharacterReceptorSelected += CharacterReceptorSelector_OnCharacterReceptorSelected;
        CharacterReceptorSelector.OnSelectsAll += CharacterReceptorSelector_OnSelectsAll;
        Hide();
    }

    private void CharacterReceptorSelector_OnCharacterSelectorStarted(object sender, CharacterReceptorSelector.OnCharacterReceptorSelectedEventArgs e)
    {
        UpdateSelectedVisual(e.characterRecepetor);
    }


    private void CharacterReceptorSelector_OnSelectsAll(object sender, CharacterReceptorSelector.OnSelectsAllEventArgs e)
    {
        if(e.characterReceptorList.Contains(_character))
        {
            Show();
        }
    }


    private void CharacterReceptorSelector_OnCharacterReceptorSelected(object sender, CharacterReceptorSelector.OnCharacterReceptorSelectedEventArgs e)
    {
        UpdateSelectedVisual(e.characterRecepetor);
    }


    private void CharacterReceptorSelector_OnCharacterSelectorCanceled(object sender, EventArgs e)
    {
        Hide();
    }

    private void CharacterReceptorSelector_OnCharacterSelectorCompleted(object sender, EventArgs e)
    {
        Hide();
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