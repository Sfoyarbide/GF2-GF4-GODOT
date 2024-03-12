using Godot;
using System;
using System.Collections.Generic;

public partial class AIManager : Node
{
    private Character _currentCharacter;
    public override void _Ready()
    {
        BattleManager.OnCurrentCharacterChanged += BattleManager_OnCurrentCharacterChanged;
        CharacterReceptorSelector.OnAISearchingReceptorListReady += CharacterReceptorSelector_OnEnemySearchingReceptorListReady;
    }

    private void BattleManager_OnCurrentCharacterChanged(object sender, BattleManager.OnCurrentCharacterChangedEventArgs e)
    {
        _currentCharacter = e.currentCharacter;
    }

    private void CharacterReceptorSelector_OnEnemySearchingReceptorListReady(object sender, CharacterReceptorSelector.OnAISearchingReceptorListReadyEventArgs e)
    {

    }

    public void AIAction(CharacterEnemy characterAI, List<Character> receptorList)
    {
        switch(characterAI.DataContainer.SelectedAction)
        {
            case MeleeAction attackAction:
                // Do logic based on his condicional state.
                break;
            case SkillAction skillAction:
                // Do logic based on his skill and properties, condicional state.
                break;
        }
    }
}