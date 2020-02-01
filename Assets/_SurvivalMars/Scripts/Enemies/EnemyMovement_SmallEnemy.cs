using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement_SmallEnemy : EnemyMovement_Base
{
    public override void IdleMovement()
    {
        print("Idle");
    }

    public override void MoveToLastKnownPosition()
    {
        throw new System.NotImplementedException();
    }

    public override void MoveToPlayer()
    {
        print("Chase");
    }

    public override void RunAway()
    {
        print("Run");
    }
}
