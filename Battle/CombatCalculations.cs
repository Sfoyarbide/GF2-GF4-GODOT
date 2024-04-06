using Godot;
using System;
using System.Collections.Generic;

public partial class CombatCalculations
{
    public static bool IsHitCalculation(Character character, Character receptor)
    {
        int agCharacter = character.DataContainer.Ag;
        int agReceptor = receptor.DataContainer.Ag;
        int luCharacter = character.DataContainer.Lu;
        int luReceptor = receptor.DataContainer.Lu;
        float accurracyModifierCharacter = character.DataContainer.AccuracyModifier;
        int randomHitChanceModifier = GD.RandRange(0, Mathf.Max(0, (luReceptor / 2) - luCharacter));
        // The formula to get the chance to hit.
        int hitChance = Mathf.Max(1, agReceptor - agCharacter + randomHitChanceModifier);

        // The formula to get the result of the chance.
        int randomHitModifier = GD.RandRange(0, Mathf.Max(1, agCharacter - agReceptor));
        int baseNumber = Mathf.RoundToInt(randomHitModifier * accurracyModifierCharacter);
        int randomValue = GD.RandRange(baseNumber, hitChance);
        return randomValue >= hitChance;
    }

    public static bool IsCriticHitCalculation(Character character, Character receptor)
    {
        int agCharacter = character.DataContainer.Ag;
        int agReceptor = receptor.DataContainer.Ag;
        int luCharacter = character.DataContainer.Lu;
        int luReceptor = receptor.DataContainer.Lu;
        float accurracyModifierCharacter = character.DataContainer.AccuracyModifier;
        // Formula for the critic change.
        int criticChance = Mathf.Max(1, (luReceptor - luCharacter) + (agReceptor - agCharacter));
        // Formula for the making the critic.
        int randomCriticModifier  = Mathf.Max(0, luCharacter - luReceptor);
        int baseNumber = Mathf.RoundToInt(randomCriticModifier * accurracyModifierCharacter);
        int randomValue = GD.RandRange(baseNumber, criticChance);
        return randomValue >= criticChance;
    }

    public static int DamageCalculation(Character character, Character receptor, Attack attack)
    {
        int statBonus = 0;
        switch(attack)
        {
            case Skill:
                statBonus = character.DataContainer.Ma;
                break;
            case MeleeAttack:
                statBonus = character.DataContainer.St;
                break;
        }

        float baseDamage = BaseDamageCalculation(attack, statBonus);
        //GD.Print("Base Damage: " + baseDamage);
        float elementMultipler = ElementMultipler(attack.AttackType, receptor);
        //GD.Print("Element Multipler: " + elementMultipler);
        float defenseMultipler = DefenseMultipler(receptor, attack.Damage, statBonus);
        //GD.Print("Defense Multipler: " + defenseMultipler);
        float randomizeMultipler = RandomizeMultipler();
        //GD.Print("Randomize Multipler: " + randomizeMultipler);
        float attackModifier = character.DataContainer.AttackModifier;
        
        float damage = baseDamage * elementMultipler * defenseMultipler * randomizeMultipler * attackModifier;
        return Mathf.RoundToInt(damage);
    }

    private static float BaseDamageCalculation(Attack attack, int statBonus)
    {
        float damage = attack.Damage;
        return damage + statBonus;
    }

    private static float ElementMultipler(AttackTypes attackType, Character receptor)
    {
        float elementMultipler = 0;

        ElementStatus elementStatus;
        if(!receptor.DataContainer.AttackElementStatusDictionary.ContainsKey(attackType))
        {
            elementStatus = ElementStatus.Normal; 
        }
        else
        {
            elementStatus = receptor.DataContainer.AttackElementStatusDictionary[attackType];
        }

        switch(elementStatus)
        {
            case ElementStatus.Normal:
                elementMultipler = 1f;
                break;
            case ElementStatus.Weakness:
                elementMultipler = 1.25f;
                break;
            case ElementStatus.Resistance:
                elementMultipler = 0.75f;
                break;
            case ElementStatus.Null:
                elementMultipler = 0;
                break;
            case ElementStatus.Absorb:
                elementMultipler = -1f;
                break;
        }

        return elementMultipler;
    }

    private static float DefenseMultipler(Character receptor, int attackBaseDamage, int statBonus)
    {
        float attackDamage = attackBaseDamage;
        float defenseModifier =  receptor.DataContainer.DefenseModifier;
        float defense = attackDamage / ((attackDamage + receptor.DataContainer.ArmorDefense + receptor.DataContainer.Co) * defenseModifier);
        return defense;
    }

    private static float RandomizeMultipler()
    {
        return (float)GD.RandRange(0.9, 1f);
    }

    public static List<Character> ObtainCharacterListByIsEnemy(List<Character> characterTurnList, bool isEnemy)
    {
        List<Character> characterListByIsEnemy = new List<Character>();
        foreach(Character character in characterTurnList)
        {
            if(character.DataContainer.IsEnemy == isEnemy)
            {
                characterListByIsEnemy.Add(character);
            }
        }
        return characterListByIsEnemy;
    }

    public static int AllOutAttackDamageCalculation(List<Character> characterList, List<Character> receptorList)
    {
        float baseDamage = 0;
        float defenseCumulativeModifier = 0;
        foreach(Character character in characterList)
        {
            baseDamage += (BaseDamageCalculation(character.DataContainer.MeleeAttack, character.DataContainer.St) * character.DataContainer.AttackModifier * RandomizeMultipler());
        }

        foreach(Character character in receptorList)
        {
            defenseCumulativeModifier = DefenseMultipler(character, (int)baseDamage, character.DataContainer.Co);
        }

        baseDamage *= defenseCumulativeModifier;
        float damage = Mathf.Max(1, baseDamage);
        return Mathf.RoundToInt(damage);
    }

    public static int IndividualPressionDamageCalculation(Character character, Character receptor)
    {
        float baseDamage = 0;
        float defenseCumulativeModifier = 0;

        baseDamage = (character.DataContainer.MeleeAttack.Damage + character.DataContainer.St + character.DataContainer.Ma) * character.DataContainer.AttackModifier * RandomizeMultipler();
        defenseCumulativeModifier = DefenseMultipler(character, (int)baseDamage, character.DataContainer.Co);

        baseDamage *= defenseCumulativeModifier;
        float damage = Mathf.Max(1, baseDamage);
        return Mathf.RoundToInt(damage);
    }

	public static void SetXpForNextLevel(CharacterData data)
    {
        data.XpForNextLevel = GetXpForNextLevel(data.Lv);
    }

    public static void AddXpGainInBattle(int xpGainForBattle, List<CharacterData> dataList)
    {
        for(int x = 0; x < dataList.Count; x++)
        {
            int newXp = dataList[x].Xp + xpGainForBattle;
            dataList[x].Xp = newXp;
            IsLevelUp(dataList[x]);
        }
    }

    public static int XpGainForBattle(List<CharacterData> dataAIList)
    {
        float xpSumUp = 0;
        float exponent = 1.5f;

        foreach(CharacterData dataAI in dataAIList)
        {
            xpSumUp += Mathf.Pow(dataAI.Xp, exponent); 
        }

        int xpGainForBattle = Mathf.RoundToInt(xpSumUp);
        return xpGainForBattle;
    }

    public static int GetXpForNextLevel(int level)
    {
        float exponent = 1.5f; // Exponent for the formula, the bigger the number, the more time will be spend.
        float baseExp = 100f; // Starting point.
        return Mathf.RoundToInt(baseExp * (Mathf.Pow(level, exponent))); // The formula for next xp.
    }

    public static bool IsLevelUp(CharacterData data)
    {
        if(data.Xp >= GetXpForNextLevel(data.Lv)) // Check if the necessary xp is reach.
        {
            LevelUp(data);
            return true;
        }
        return false;
    }

    public static void LevelUp(CharacterData data)
    {
        data.Lv += 1; // Level Up
        data.IncreaseHpMax();
        data.IncreaseSpMax();
        int howManyStatToIncrease = GD.RandRange(1, 3); // Numbers of stats to increase. 

        for(int x = 0; x < howManyStatToIncrease; x++)
        {
            int indexStat = GD.RandRange(0, 5); // index stat to obtain.
            switch(indexStat)
            {
                case 0:
                    data.St = data.IncreaseStat(data.St);
                    break;
                case 1:
                    data.Ma = data.IncreaseStat(data.Ma);
                    break;
                case 2:
                    data.Ag = data.IncreaseStat(data.Ag);
                    break;
                case 3:
                    data.Co = data.IncreaseStat(data.Co);
                    break;
                case 4:
                    data.Lu = data.IncreaseStat(data.Lu);
                    break;
            }
        }

        data.Xp = 0;
    }

    public static int MoveTheIndex(int min, int max, int index)
    {
        int newIndexValue = index;
        if(Input.IsActionJustPressed("left"))
        {
            if(index > min)
            {
                newIndexValue--;
            }
            else
            {
                newIndexValue = max;
            }
        }
        if(Input.IsActionJustPressed("right"))
        { 
            if(index < max)
            {
                newIndexValue++;
            }
            else
            {
                newIndexValue = min;
            }
        }
        return newIndexValue;
    }

    public static Character GetLowestHpCharacterInCharacterList(List<Character> characterList)
    {
        Character characterWithLowestHp = characterList[0];
        int lowestHp = characterList[0].DataContainer.Hp;

        foreach(Character character in characterList)
        {
            int newHp = character.DataContainer.Hp;
            if(newHp < lowestHp)
            {
                lowestHp = newHp;
                characterWithLowestHp = character;
            }
        }

        return characterWithLowestHp;
    }

    public static Character GetRandomCharacterInCharacterList(List<Character> characterList)
    {
        int randomIndex = GD.RandRange(0, characterList.Count-1);
        
        Character randomCharacter = characterList[randomIndex];

        return randomCharacter;
    }
}
