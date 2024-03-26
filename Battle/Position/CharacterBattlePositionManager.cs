using Godot;
using System;
using System.Collections.Generic;

public partial class CharacterBattlePositionManager : Node
{
    private Node _advantage;
    private Node _disadvantage;

    private List<Node3D> _enemyAdvantageStartPos;
    private List<Node3D> _enemyAdvantageEndPos;

    public override void _Ready()
    {
        // Finding necessary nodes.
        _advantage = GetNode<Node>("Advantage");
        //_disadvantage

        // Finding position's father node.
        Node enemyAdvantageStartPositions = _advantage.GetNode<Node>("Enemy/StartPositions");
        Node enemyAdvantageEndPositions = _advantage.GetNode<Node>("Enemy/EndPositions");

        // Adding the positions to the corresponding list.
        _enemyAdvantageEndPos = AddPositionsInList(enemyAdvantageEndPositions);
        _enemyAdvantageStartPos = AddPositionsInList(enemyAdvantageStartPositions);
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
}
