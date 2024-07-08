using Godot;
using System;

public abstract partial class GameBaseState : Node
{
    private GameDirector _context;
    public GameDirector Context { get { return _context; } set { _context = value; } }
    public abstract void EnterState(GameDirector context);
    public abstract void UpdateState(float delta);
    public abstract void ExitState();
}