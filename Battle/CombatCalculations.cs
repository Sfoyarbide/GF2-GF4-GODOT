using Godot;
using System;
using System.Collections.Generic;

public partial class CombatCalculations
{
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

	public static void SetXpForNextLevel(CharacterDataResource data)
    {
        data.XpForNextLevel = GetXpForNextLevel(data.Lv);
    }

    public static void AddXpGainInBattle(int xpGainForBattle, List<CharacterDataResource> dataList)
    {
        for(int x = 0; x < dataList.Count; x++)
        {
            int newXp = dataList[x].Xp + xpGainForBattle;
            dataList[x].Xp = newXp;
            IsLevelUp(dataList[x]);
        }
    }

    public static int XpGainForBattle(List<CharacterDataResource> dataAIList)
    {
        float xpSumUp = 0;
        float exponent = 1.5f;

        foreach(CharacterDataResource dataAI in dataAIList)
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

    public static bool IsLevelUp(CharacterDataResource data)
    {
        if(data.Xp >= GetXpForNextLevel(data.Lv)) // Check if the necessary xp is reach.
        {
            LevelUp(data);
            return true;
        }
        return false;
    }

    public static void LevelUp(CharacterDataResource data)
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
