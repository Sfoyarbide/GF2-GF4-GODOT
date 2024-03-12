using Godot;
using System;
using System.Collections.Generic;

public abstract partial class CharacterEnemy : Character
{
    // Scripted Pattern.
    public static event EventHandler<OnEnemySearchingReceptorEventArgs> EnemySearchingReceptorList;
    public class OnEnemySearchingReceptorEventArgs : EventArgs
    {
        public List<ReceptorCriteria> receptorCriteriaList;
    }

    private int _turnEnemyPassed = 0;

    public int TurnEnemyPassed { get {return _turnEnemyPassed; } set {_turnEnemyPassed = value;}}

    public abstract void Pattern();
    public abstract void Randomize();
    public abstract void Condicional();
    public void ResetPattern()
    {
        _turnEnemyPassed = 0;
    }

    public void OnEnemySearchingReceptorList(OnEnemySearchingReceptorEventArgs e)
    {
        EnemySearchingReceptorList?.Invoke(this, e);
    }
}