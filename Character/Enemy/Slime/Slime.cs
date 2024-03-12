using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Slime : CharacterEnemy
{
    public override void Condicional()
    {
        
    }

    public override void Pattern()
    {
        if(TurnEnemyPassed == 0)
        {
            DataContainer.SelectedAction = DataContainer.ActionList[0];
            List<ReceptorCriteria> receptorCriteriaList = new List<ReceptorCriteria>();
            receptorCriteriaList.Add(ReceptorCriteria.Enemy);
            OnEnemySearchingReceptorList(new OnEnemySearchingReceptorEventArgs{
                receptorCriteriaList = receptorCriteriaList,
            });
        }
    }

    public override void Randomize()
    {
        
    }
}