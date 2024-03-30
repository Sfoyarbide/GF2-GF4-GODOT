using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyGroupInLevel1 : EnemyGroupInLevel
{
    public override void _Ready()
    {
        EnemyGroupInLevelList.Add(new List<string>{
            "Slime"
        });
        EnemyGroupInLevelList.Add(new List<string>{
            "Slime","Slime"
        });
        EnemyGroupInLevelList.Add(new List<string>{
            "Slime","Slime","Slime"
        });
    }
}
