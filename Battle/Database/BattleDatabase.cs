using Godot;
using System;

public partial class BattleDatabase : Node
{
	private BattleManager _battleManager;
	private CharacterReceptorSelector _characterReceptorSelector;
	private Inventory _inventory;
	private SkillDatabase _skillDatabase;

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
    	_battleManager = GetTree().Root.GetNodeOrNull<BattleManager>("Level1/Battle");	
		_characterReceptorSelector = GetTree().Root.GetNodeOrNull<CharacterReceptorSelector>("Level1/Battle/ManagerContainer/CharacterReceptorSelector");	
		_inventory = GetNode<Inventory>("Inventory");
		_skillDatabase = GetNode<SkillDatabase>("SkillDatabase");
	}

	public Character GetCurrentCharacter()
	{
		return _battleManager.GetCurrentCharacter();
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
}