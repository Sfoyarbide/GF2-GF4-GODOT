using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyGroupInLevel : Node
{
    private List<List<string>> _enemyGroupInLevelList = new List<List<string>>();

    public List<List<string>> EnemyGroupInLevelList 
    {
        get {return _enemyGroupInLevelList;} 
        protected set {_enemyGroupInLevelList = value;}
    }
}
