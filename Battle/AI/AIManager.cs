using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class AIManager : Node
{
    private CharacterEnemy _currentCharacter;
    private Dictionary<Character, float> _characterValueDictionary = new Dictionary<Character, float>();

    public static event EventHandler<OnAIReadyToDoActionEventArgs> OnAIReadyToDoAction;
    public class OnAIReadyToDoActionEventArgs : EventArgs
    {
        public Attack attack;
        public List<Character> receptorList;
    }

    public override void _Ready()
    {
        // Subcribing to events. 
        BaseAction.AttackState += BaseAction_AttackState;
        BattleStarter.OnBattleCharacterSetupFinished += BattleStarter_OnBattleCharacterSetupFinished;
        BattleManager.OnBattleEnd += BattleManager_OnBattleEnd;
        BattleManager.OnCurrentCharacterChanged += BattleManager_OnCurrentCharacterChanged;
        CharacterReceptorSelector.OnEnemySearchingReceptorListReady += CharacterReceptorSelector_OnEnemySearchingReceptorListReady;
        CharacterData.OnDie += CharacterData_OnDie;
        CharacterData.OnRevive += CharacterData_OnRevive;
    }

    // The function that updates the priority/value of the characters inside the dictionary.
    private void UpdateCharacterInValueDictionary(Character sender, Character receptor, int damage)
    {
        if(sender is not CharacterEnemy)
        {
            _characterValueDictionary[sender] += 0.5f + (damage / 100);
        }

        if(receptor is not CharacterEnemy)
        {
            // Subtracting value or priority for the AI calculation.

            if(damage == 0)
            {
                return;
            }

            _characterValueDictionary[receptor] -= 0.25f + (damage / 100);
            if(_characterValueDictionary[receptor] < 0)
            {
                _characterValueDictionary[receptor] = 0;
            }
        }
        else
        {
            // Adding value or priority for the healing in the AI calculation.
            // Here adding is negative, because is the way of knowing how receive more damage.
            _characterValueDictionary[receptor] -= 0.15f + (damage / 100);
        }
    }

    private List<Character> GetReceptorListAI(Attack attack, List<Character> characterReceptorList)
    {
        List<Character> finalReceptorList = new List<Character>();
        if(attack.AllReceptors)
        {
            finalReceptorList = characterReceptorList;
            return finalReceptorList;
        }

        Character receptor = null;

        switch(attack)
        {
            case ModifierSkill modifierSkill:
                receptor = SelectionBasedOnModifierSkill(modifierSkill, characterReceptorList);
                break;
            case HealSkill:
                receptor = GetLowestValueCharacter(characterReceptorList);
                break;
            default:
                receptor = SelectionBasedOnAgresive(attack, characterReceptorList);
                break;
        }

        finalReceptorList.Add(receptor);

        return finalReceptorList;
    }

    private Character SelectionBasedOnModifierSkill(ModifierSkill modifierSkill, List<Character> characterReceptorList)
    {
        Character receptor = null;
        if(modifierSkill.SkillType == SkillType.ModifierUP)
        {
            switch(modifierSkill)
            {
                case AttackUPSkill:
                    receptor = GetHighestValueCharacter(characterReceptorList);
                    break;
                case DefenseUPSkill:
                    receptor = GetLowestValueCharacter(characterReceptorList);
                    break;
                case AgilityUPSkill:
                    receptor = CombatCalculations.GetRandomCharacterInCharacterList(characterReceptorList);
                    break; 
            }
        }
        
        if(modifierSkill.SkillType == SkillType.ModifierDOWN)
        {
            switch(modifierSkill)
            {
                case AttackDOWNSkill:
                    receptor = GetHighestValueCharacter(characterReceptorList);
                    break;
                case DefenseDOWNSkill:
                    receptor = GetLowestValueCharacter(characterReceptorList);
                    break;
                case AgilityDOWNSkill:
                    receptor = CombatCalculations.GetRandomCharacterInCharacterList(characterReceptorList);
                    break; 
            }
        }

        return receptor;
    }

    private Character SelectionBasedOnAgresive(Attack attack, List<Character> characterReceptorList)
    {
        Character receptor = null;
        switch(_currentCharacter.EnemyType)
        {
            case EnemyType.HighAgresive:
                receptor = GetHighestValueCharacter(characterReceptorList);
                break;
            case EnemyType.MidAgresive:
                receptor = CombatCalculations.GetRandomCharacterInCharacterList(characterReceptorList);
                break;
            case EnemyType.LowAgresive:
                receptor = GetLowestValueCharacter(characterReceptorList);
                break;
        }
        return receptor;
    }

    private Character GetLowestValueCharacter(List<Character> characterReceptorList)
    {
        float currentValue = 9999;
        Character lowestCharacter = null; 
        foreach(Character character in characterReceptorList)
        {
            float value = _characterValueDictionary[character];
            if(value < currentValue)
            {
                lowestCharacter = character;
                currentValue = value;
            }
        }
        return lowestCharacter;
    }

    private Character GetHighestValueCharacter(List<Character> characterReceptorList)
    {
        float currentValue = -9999;
        Character highestCharacter = null; 
        foreach(Character character in characterReceptorList)
        {
            float value = _characterValueDictionary[character];
            if(value > currentValue)
            {
                highestCharacter = character;
                currentValue = value;
            }
        }
        return highestCharacter;
    }

    private void BaseAction_AttackState(object sender, BaseAction.AttackStateEventArgs e)
    {
        UpdateCharacterInValueDictionary(e.current, e.receptor, e.damage);
    }

    private void BattleStarter_OnBattleCharacterSetupFinished(object sender, BattleStarter.OnBattleCharacterSetupFinishedEventArgs e)
    {
        foreach(Character character in e.characterTurnList)
        {
            _characterValueDictionary.Add(character, 0);
        }
    }

    private void BattleManager_OnCurrentCharacterChanged(object sender, BattleManager.OnCurrentCharacterChangedEventArgs e)
    {
        if(e.currentCharacter.DataContainer.IsEnemy)
        {
            _currentCharacter = (CharacterEnemy)e.currentCharacter;
        }
    }

    private void CharacterData_OnDie(object sender, CharacterData.CharacterDataEventArgs e)
    {
        _characterValueDictionary.Remove(e.character);
    }

    private void CharacterData_OnRevive(object sender, CharacterData.CharacterDataEventArgs e)
    {
        _characterValueDictionary.Add(e.character, 0);
    }

    private void BattleManager_OnBattleEnd(object sender, BattleManager.OnBattleEndEventArgs e)
    {
        _characterValueDictionary.Clear();
    }

    private void CharacterReceptorSelector_OnEnemySearchingReceptorListReady(object sender, CharacterReceptorSelector.OnEnemySearchingReceptorListReadyEventArgs e)
    {
        List<Character> characterReceptorList = GetReceptorListAI(e.attack, e.characterReceptorList);
        OnAIReadyToDoAction(this, new OnAIReadyToDoActionEventArgs{
            attack = e.attack,
            receptorList = characterReceptorList
        });
    }
}