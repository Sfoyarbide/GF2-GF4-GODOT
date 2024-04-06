using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public abstract partial class CharacterEnemy : Character
{
    // Vars.
    [Export]
    private EnemyType _enemyType;

    private Node _attackContainer;
    
    private Attack _currentAttack;
    private List<Attack> _attackList = new List<Attack>();

    private int _turnEnemyPassed = 0;

    // Events.
    public static event EventHandler<OnEnemySearchingReceptorEventArgs> EnemySearchingReceptorList;
    public class OnEnemySearchingReceptorEventArgs : EventArgs
    {
        public Attack attack;
        public Character currentCharacter;
    }

    // Getters and Setters.
    public EnemyType EnemyType { get {return _enemyType; } set {_enemyType = value;}}
    public Attack CurrentAttack { get {return _currentAttack; } set {_currentAttack = value;}}
    public List<Attack> AttackList = new List<Attack>();
    public int TurnEnemyPassed { get {return _turnEnemyPassed; } set {_turnEnemyPassed = value;}}

    public override void _Ready()
    {
        // Finding nodes.
        base._Ready();

        // Subcribing to events.
        BattleManager.OnBattleStart += BattleManager_OnBattleStart;
        BattleManager.OnCurrentCharacterChanged += BattleManager_OnCurrentCharacterChanged;
        CharacterData.OnDie += CharacterData_OnDie;
        CharacterReceptorSelector.OnSelectionFailed += CharacterReceptorSelector_OnSelectionFailed;
    }

    protected Attack FindAttack(string attackName)
    {
        foreach(Attack attack in _attackList)
        {
            if(attack.AttackName == attackName)
            {
                return attack;
            }
        }

        return null;
    }

    protected Skill FindSkillByType(SkillType skillType)
    {
        foreach(Skill skill in _attackList)
        {
            if(skill.SkillType == skillType)
            {
                return skill;
            }
        }

        return null;
    }

    public void OnEnemySearchingReceptorList()
    {
        EnemySearchingReceptorList?.Invoke(this, new OnEnemySearchingReceptorEventArgs{
            attack = _currentAttack,
            currentCharacter = this
        });
    }

    // Here is where the enemy pattern resides.
    protected abstract void Pattern();

    protected abstract void OnFailedSelection(Attack attack);

    protected Attack RandomizeAttack()
    {
        int randomIndex = GD.RandRange(0, _attackList.Count -1);
        return _attackList[randomIndex];
    }

    protected void CheckResetPattern(int maxTurnPassed)
    {
        if(TurnEnemyPassed > maxTurnPassed)
        {
            TurnEnemyPassed = 0;
        }
    }

    private void BattleManager_OnCurrentCharacterChanged(object sender, BattleManager.OnCurrentCharacterChangedEventArgs e)
    {
        if(e.currentCharacter == this)
        {
            Pattern();
        }
    }

    private void BattleManager_OnBattleStart(object sender, BattleManager.OnBattleStartEventArgs e)
    {
        if(e.enemyList.Contains(this))
        {
            _attackList.Add(DataContainer.MeleeAttack);
            _attackList.AddRange(DataContainer.SkillList);
        }
    }

    private void CharacterData_OnDie(object sender, CharacterData.CharacterDataEventArgs e)
    {
        _attackList.Clear();
    }

    private void CharacterReceptorSelector_OnSelectionFailed(object sender, CharacterReceptorSelector.ReceptionSelectorEventArgs e)
    {
        if(e.currentCharacter == this)
        {
            GD.Print("On failed Selection AI");
            OnFailedSelection(CurrentAttack);
        }
    }
}

public enum EnemyType
{
    HighAgresive,
    MidAgresive,
    LowAgresive
}