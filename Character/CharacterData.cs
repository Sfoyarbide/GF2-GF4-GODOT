using Godot;
using System;
using System.Collections.Generic;

public partial class CharacterData : Node
{
    private Character _character;

    // Stats

    private int _hp;
    private int _hpMax;
    private int _sp;
    private int _spMax;
    private int _lv;
    private int _xp;
    private int _xpForNextLevel;
    private bool _isEnemy;
    private int _st;
    private int _ma;
    private int _ag; 
    private int _co;
    private int _lu;

    private List<BaseAction> _actionList = new List<BaseAction>();
    private List<Skill> _skillList = new List<Skill>();
    private BaseAction _selectedAction;
    [Export]
    private PressionLevelModifier _pressionLevelModifier;

    // Battle vars.
    private bool _isDefending;
    [Export]
    private bool _alreadyHitWeakness;
    [Export]
    private float _pressionLevel;


    // Modifiers vars.
    private float _accuracyModifier = 1; 
    private float _defenseModifier = 1;
    private float _attackModifier = 1;

    // Constant vars.
    private Dictionary<AttackTypes, ElementStatus> _attackElementStatusDictionary = new Dictionary<AttackTypes, ElementStatus>();
    private Dictionary<int, Skill> _skillGrantByLevel = new Dictionary<int, Skill>();
    private InflictStates _individualPressionInflictStateType;

    private Attack _meleeAttack;
    private int _armorDefense; // Temp!!!
    private List<ModifierStatsInflict> _modifierStatsInflictList = new List<ModifierStatsInflict>();
    private InflictState _inflictState;

    public static event EventHandler<OnHpChangedEventArgs> OnHpChanged;
    public class OnHpChangedEventArgs : EventArgs
    {
        public Character character;
        public bool isLessThanBefore;
        public int difference;
    }

    public Character Character
    {
        get{ return _character;}
        set{ _character = value;}
    }
	[Export]
    public int Hp 
    {
        get 
        {
            return _hp;
        } 
        set 
        {
            int previousHp = _hp;
            _hp = value;

            if(_hp > HpMax)
            {
                _hp = HpMax;
            }

            if(_hp < 0)
            {
                // Die.
            }

            OnHpChanged?.Invoke(this, new OnHpChangedEventArgs{
                character = _character,
                isLessThanBefore = _hp < previousHp,
                difference = previousHp - _hp
            }); 
        }
    } 
	[Export]
    public int HpMax {get {return _hpMax;} set {_hpMax = value;}} 
	[Export]
    public int Sp {get {return _sp;} set {_sp = value;}} 
	[Export]
    public int SpMax {get {return _spMax;} set {_spMax = value;}} 
	[Export]
    public int Lv {get {return _lv;} set {_lv = value;}} 
	[Export]
    public int Xp {get {return _xp;} set {_xp = value;}} 
	[Export]
    public int XpForNextLevel {get {return _xpForNextLevel;} set {_xpForNextLevel = value;}} 
	[Export]
    public bool IsEnemy {get {return _isEnemy;} set {_isEnemy = value;}} 
	[Export]
    public int St {get {return _st;} set {_st = value;}} 
	[Export]
    public int Ma {get {return _ma;} set {_ma = value;}}
	[Export]
    public int Ag {get {return _ag;} set {_ag = value;}} 
	[Export]
    public int Co {get {return _co;} set {_co = value;}}
	[Export]
    public int Lu {get {return _lu;} set {_lu = value;}}
    public Attack MeleeAttack {get {return _meleeAttack;} set {_meleeAttack = value;}}
    [Export]
    public int ArmorDefense {get {return _armorDefense;} set {_armorDefense = value;}}
    [Export]
    public float AccuracyModifier { get { return _accuracyModifier; } set {_accuracyModifier = value;}}
    [Export]
    public float DefenseModifier { get { return _defenseModifier; } set {_defenseModifier = value;}}
    [Export]
    public float AttackModifier { get { return _attackModifier; } set {_attackModifier = value;}}

    
    public List<BaseAction> ActionList {get {return _actionList; } set {_actionList = value; }}
    public List<Skill> SkillList {get {return _skillList; } set {_skillList = value; }}

    public Dictionary<AttackTypes, ElementStatus> AttackElementStatusDictionary  {get {return _attackElementStatusDictionary;} set {_attackElementStatusDictionary = value;}}
    public Dictionary<int, Skill> SkillGrantByLevel { get {return _skillGrantByLevel; } }

    public List<ModifierStatsInflict> ModifierStatsInflictList {get {return _modifierStatsInflictList;}}

    public InflictState InflictState {get { return _inflictState; } set { _inflictState = value; }} 
    public BaseAction SelectedAction {get {return _selectedAction;} set {_selectedAction = value;}}

    public PressionLevelModifier PressionLevelModifier { get {return _pressionLevelModifier; } set { _pressionLevelModifier = value; }}
    public InflictStates IndividualPressionInflictStateType { get {return _individualPressionInflictStateType; } set { _individualPressionInflictStateType = value; }}

    [Export]
    public bool IsDefending {get {return _isDefending;} set {_isDefending = value;}}

    public bool AlreadyHitWeakness {get {return _alreadyHitWeakness;} set {_alreadyHitWeakness = value;}}

    public float PressionLevel 
    {
        get { return _pressionLevel; } 
        set 
        { 
            _pressionLevel = value;
            if(_pressionLevel > 1)
            {
                _pressionLevel = 1;
            }
        }
    }

    public bool IsElementStatusToAttackType(AttackTypes attackType, ElementStatus elementStatus)
    {
        if(AttackElementStatusDictionary.TryGetValue(attackType, out ElementStatus value))
        {
            return elementStatus == value;
        }
        else
        {
            return false;
        }
    }

    public void IncreaseHpMax()
    {
        _hpMax = _hpMax + 30 + _co / 2;
    }

    public void IncreaseSpMax()
    {
        _spMax += 15 + _ma / 4;
    }

    public int IncreaseStat(int stat)
    {
        return stat += GD.RandRange(1, 3);
    }

    public override void _Ready()
    {
        BattleManager.OnTurnEnd += BattleManager_OnTurnEnd;
        _meleeAttack = GetNode<Attack>("MeleeAttack");
    }

    private void BattleManager_OnTurnEnd(object sender, EventArgs e)
    {
        AlreadyHitWeakness = false;
    }
}

public enum PressionLevelModifier
{
    Healer,
    Attack,
    ReceiveDamage
}