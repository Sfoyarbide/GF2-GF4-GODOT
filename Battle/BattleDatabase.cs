using Godot;
using System;

public partial class BattleDatabase : Node
{
	private BattleManager _battleManager;
	private CharacterReceptorSelector _characterReceptorSelector;

	public BattleManager BattleManager 
	{ 
		get { return _battleManager; } 
		set { _battleManager = value; } 
	}
	public CharacterReceptorSelector CharacterReceptorSelector 
	{
		get { return _characterReceptorSelector; } 
		set { _characterReceptorSelector = value; }
	}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    	_battleManager = GetTree().Root.GetNode<BattleManager>("Level1/BattleManager");	
		_characterReceptorSelector = GetTree().Root.GetNode<CharacterReceptorSelector>("Level1/CharacterReceptorSelector");	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
