using Godot;
using System;

public partial class GameDirector : Node
{
    private GameBaseState _currentGameState;
    private Node _stateContainer;
    public GameBaseState combatGameState;
    public GameBaseState explorationCombatGameState;

    public override void _Ready()
    {
        _stateContainer = GetNode<Node>("StateContainer");
        combatGameState = GetNode<GameBaseState>("Combat");
        explorationCombatGameState = GetNode<GameBaseState>("ExplorationCombat");
    }

    public void SwitchState(GameBaseState gameBaseState)
    {
        _currentGameState.ExitState();
        _currentGameState = gameBaseState;
        _currentGameState.EnterState(this);
    }
}