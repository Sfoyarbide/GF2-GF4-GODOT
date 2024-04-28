using Godot;
using System;

public abstract partial class PlayerExplorationBaseState : Node
{
    private PlayerExplorationMovement _context;

    public PlayerExplorationMovement Context {get {return _context;} set {_context = value;}}

    public abstract void OnEnter(PlayerExplorationMovement context);
    public abstract void OnUpdate(float delta);
}