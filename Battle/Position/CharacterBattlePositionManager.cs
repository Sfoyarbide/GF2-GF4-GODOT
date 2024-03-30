using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class CharacterBattlePositionManager : Node
{
    // Stores the values for the lerp between the start and end position.
    // The keys are the enemy count, and the values are the lerp positions.
    private Dictionary<int, List<float>> _lerpValuesForPositions = new Dictionary<int, List<float>>();

    // For knowing if we have to rearrange the positions.
    private bool _hasToRearrangeEnemy;

    // The two positions based on how the battle starts.
    private Node _advantage;
    private Node _disadvantage;

    // List of start and end positions. 
    private List<Node3D> _enemyAdvantageStartPos;
    private List<Node3D> _enemyAdvantageEndPos;

    public override void _Ready()
    {
        // Adding lerp values.
        
        // For one characters.
        _lerpValuesForPositions.Add(1, new List<float>{
            0.5f
        });

        // For two characters.
        _lerpValuesForPositions.Add(2, new List<float>{
            0.25f,
            0.75f
        });

        // For three characters.
        _lerpValuesForPositions.Add(3, new List<float>{
            0.25f,
            0.5f,
            0.75f
        });

        // For four characters.
        _lerpValuesForPositions.Add(4, new List<float>{
            0,
            0.25f,
            0.5f,
            0.75f,
            1 
        });

        // Finding necessary nodes.
        _advantage = GetNode<Node>("Advantage");
        //_disadvantage

        // Finding position's father node.
        Node enemyAdvantageStartPositions = _advantage.GetNode<Node>("Enemy/StartPositions");
        Node enemyAdvantageEndPositions = _advantage.GetNode<Node>("Enemy/EndPositions");

        // Adding the positions to the corresponding list.
        _enemyAdvantageEndPos = AddPositionsInList(enemyAdvantageEndPositions);
        _enemyAdvantageStartPos = AddPositionsInList(enemyAdvantageStartPositions);

        // Subcribing to events.
        CharacterData.OnDie += CharacterData_OnDie;
        BattleManager.OnBattleStart += BattleManager_OnBattleStart;
        BattleManager.OnCurrentCharacterChanged += BattleManager_OnCurrentCharacterChanged;
    }

    private void SendCharacterToWaitArea(Character character)
    {
        character.Hide();
        character.GlobalPosition = new Vector3(0,-10,0);
    }

    // Auxiliary Method for helping to add the positions in the lists.

    private List<Node3D> AddPositionsInList(Node father)
    {
        List<Node3D> positions = new List<Node3D>();
        
        for(int x = 0; x < father.GetChildCount(); x++)
        {
            Node3D position = (Node3D)father.GetChild(x);
            positions.Add(position);
        }

        return positions;
    }

    // Method that rearreange the characters.
    private void RearrangeCharacterList(List<Character> characterList, List<Node3D> startPositions, List<Node3D> endPositions)
    {
        List<int> characterListCountList = new List<int>();
        int characterListCountIndex = 0;

        // This means that they are 2 rows.
        if(characterList.Count > 4)
        {
            characterListCountList.Add(4);
            characterListCountList.Add(characterList.Count - 4);
        }
        else
        {
            characterListCountList.Add(characterList.Count);
        }

        // Index for both lists
        int positionIndex = 0;  
        int lerpValueIndex = 0;
        // For knowing how many characters we already rearrange.

        List<float> lerpValues = new List<float>();

        foreach(Character character in characterList)
        {
            Vector3 startPosition = startPositions[positionIndex].GlobalPosition;
            Vector3 endPosition = endPositions[positionIndex].GlobalPosition;

            lerpValues = _lerpValuesForPositions[characterListCountList[characterListCountIndex]];

            float finalLerpValue = lerpValues[lerpValueIndex];

            if(finalLerpValue != 0)
            {
                if(finalLerpValue > 0)
                {
                    finalLerpValue += character.CombatPositionModifier;
                }
                else if (finalLerpValue < 0)
                {
                    finalLerpValue -= character.CombatPositionModifier;
                }
            }

            character.GlobalPosition = startPosition.Lerp(endPosition, finalLerpValue);

            lerpValueIndex++;

            // Means that we had passed the maximum values for the row, passing to the other.
            if(lerpValueIndex == 3)
            {
                lerpValues.Clear();
                characterListCountIndex++;
                lerpValueIndex = 0;
            }
        }
    }

    private void CharacterData_OnDie(object sender, CharacterData.CharacterDataEventArgs e)
    {
        if(e.character.DataContainer.IsEnemy)
        {
            // Send enemy to an invisible place.
            SendCharacterToWaitArea(e.character);
            _hasToRearrangeEnemy = true;
        }
    }

    private void BattleManager_OnBattleStart(object sender, BattleManager.OnBattleStartEventArgs e)
    {
        RearrangeCharacterList(e.enemyList, _enemyAdvantageStartPos, _enemyAdvantageEndPos);
    }

    private void BattleManager_OnCurrentCharacterChanged(object sender, BattleManager.OnCurrentCharacterChangedEventArgs e)
    {
        // In case of advantage
        if(_hasToRearrangeEnemy)
        {
            RearrangeCharacterList(e.enemyList, _enemyAdvantageStartPos, _enemyAdvantageEndPos);
        }
    }
}
