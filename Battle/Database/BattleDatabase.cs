using Godot;
using System;

public partial class BattleDatabase : Node
{
	private BattleManager _battleManager;
	private CharacterReceptorSelector _characterReceptorSelector;
	private Inventory _inventory;
	private BattleStarter _battleStarter;
	private DebugTools_Battle _debugTools_Battle;
	private SkillDatabase _skillDatabase;

	public BattleManager BattleManager 
	{ 
		get { return _battleManager; } 
		set 
		{ 
			_battleManager = value; 
		} 
	}
	public CharacterReceptorSelector CharacterReceptorSelector 
	{
		get { return _characterReceptorSelector; } 
		set 
		{
			_characterReceptorSelector = value;
		}
	}
	public BattleStarter BattleStarter
	{
		get { return _battleStarter;}
		set 
		{
			_battleStarter = value; 
		}
	}
	public Inventory Inventory
	{
		get { return _inventory; }
		set { _inventory = value; }
	}
	public SkillDatabase SkillDatabase 
	{
		get { return _skillDatabase; }
	}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
		// DungeonExplorationGym/
    	_battleManager = GetTree().Root.GetNode<BattleManager>("DungeonExplorationGym/Level1/Battle");	
		_characterReceptorSelector = GetTree().Root.GetNode<CharacterReceptorSelector>("DungeonExplorationGym/Level1/Battle/ManagerContainer/CharacterReceptorSelector");	
		_battleStarter = _battleManager.GetNode<BattleStarter>("BattleStarter");
		_debugTools_Battle = GetTree().Root.GetNode<DebugTools_Battle>("DungeonExplorationGym/DebugToolBattleUI/MarginContainer/TabContainer/Battle");
		_inventory = GetNode<Inventory>("Inventory");
		_skillDatabase = GetNode<SkillDatabase>("SkillDatabase");
	}

	public Character GetCurrentCharacter()
	{
		return _battleManager.GetCurrentCharacter();
	}

}
