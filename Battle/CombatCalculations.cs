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
        int randomHitChanceModifier = GD.RandRange(0, Mathf.Max(0, luReceptor - luCharacter));
        // The formula to get the chance to hit.
        int hitChance = Mathf.Max(1, agReceptor - agCharacter + randomHitChanceModifier);

        // The formula to get the result of the chance.
        int randomHitModifier = GD.RandRange(0, Mathf.Max(1, agReceptor - agCharacter));
        int randomValue = GD.RandRange(0 + randomHitModifier, hitChance);
        return randomValue >= hitChance;
    }

    public static bool IsCriticHitCalculation(Character character, Character receptor)
    {
        int agCharacter = character.DataContainer.Ag;
        int agReceptor = receptor.DataContainer.Ag;
        int luCharacter = character.DataContainer.Lu;
        int luReceptor = receptor.DataContainer.Lu;
        // Formula for the critic change.
        int criticChance = Mathf.Max(1, ((luReceptor - luCharacter) / agCharacter) + agReceptor);
        int randomCriticModifier  = Mathf.Max(0, luReceptor - luCharacter);
        // Formula for the making the critic.
        int randomValue = GD.RandRange(0 + randomCriticModifier, criticChance);
        return randomValue == criticChance;
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

        float baseDamage = BaseDamageCalculation(character, attack, statBonus);
        //GD.Print("Base Damage: " + baseDamage);
        float elementMultipler = ElementMultipler(attack.AttackType, receptor);
        //GD.Print("Element Multipler: " + elementMultipler);
        float defenseMultipler = DefenseMultipler(receptor, attack, statBonus);
        //GD.Print("Defense Multipler: " + defenseMultipler);
        float randomizeMultipler = RandomizeMultipler();
        //GD.Print("Randomize Multipler: " + randomizeMultipler);
        
        float damage = baseDamage * elementMultipler * defenseMultipler * randomizeMultipler;
        return Mathf.RoundToInt(damage);
    }

    private static float BaseDamageCalculation(Character character, Attack attack, int statBonus)
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

    private static float DefenseMultipler(Character receptor, Attack attack, int statBonus)
    {
        float attackDamage = attack.Damage;
        float defense = attackDamage / (attackDamage + receptor.DataContainer.ArmorDefense + receptor.DataContainer.Ma);
        return defense;
    }

    private static float RandomizeMultipler()
    {
        return (float)GD.RandRange(0.9, 1f);
    }

    //Legacy Calculations
	public static int CalculateDamage(int baseDamage, int defense) // Calculation for damage.
    {
        int damage;
        if(baseDamage >= defense) // Check if the damage is more or equal to the defense
        {
            damage = baseDamage * 2 - defense; 
        }
        else
        {
            damage = baseDamage * baseDamage / defense;
        }
        return damage;
    }
    public static bool CheckIsHit(int StatCharacter, int StatCharacterReceptor, int dice) // Calculation to check if it's hit.
    {
        if((StatCharacter * dice) - StatCharacterReceptor > StatCharacterReceptor * 2) // The formula to check if you hit.
        {
            return true;
        }
        else
        {
            return false;
        }
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
}
