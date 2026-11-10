//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.AI;

//public class Boss_BT : Behavior_Tree
//{
//    //Composantes
//    GameObject self;
//    [SerializeField] List<Transform> center;

//    //Tree Actions 
//    //ExplodeAround_Action explodeAround_Action;
//    //ExplodeAround_Action crazyExplodeAround_Action;
//    //FireAround_Action fireAround_Action;
//    //AimPlayers_Action aimPlayers_Action;
//    GoCrazy_Action goCrazy_Action;
//    //SpinningBowl_Action spinningBowl_Action;
//    Wait_Action wait_Action;
//    MoveToPoint_Action moveToCenter_Action;
//    Wander_Action wander_Action;

//    //Tree Conditions
//    IsInRange_Condition isInRange_Condition;
//    HasVision_Condition hasVision_Condition;
//    Seconds_Condition seconds_Condition;
//    //Tree Composites
//    Behavior_Composite sequence_1;
//    Behavior_Composite parallel_1;
//    Behavior_Composite sequence_2;
//    public override void InitializeTree()
//    {
//        if (!IsServer)
//        {
//            return;
//        }


//        self = this.gameObject;

//        //Conditions 
//        isInRange_Condition = new IsInRange_Condition(self, 30, false);
//        seconds_Condition = new Seconds_Condition(30,false);
       
        
//        //Actions
//        crazyExplodeAround_Action = new ExplodeAround_Action(new Behavior_Condition[] {seconds_Condition}, self, 24,2 );
//        explodeAround_Action = new ExplodeAround_Action(null, self, 8, 1);
//        fireAround_Action = new FireAround_Action(new Behavior_Condition[] { isInRange_Condition }, self, 360);
//        aimPlayers_Action = new AimPlayers_Action(new Behavior_Condition[] { isInRange_Condition }, self, 3, 0.1f);
//        spinningBowl_Action = new SpinningBowl_Action(null, self, 0,9);
//        goCrazy_Action = new GoCrazy_Action(null, self, self.GetComponent<NavMeshAgent>());
//        wait_Action = new Wait_Action(null, 0.5f);
//        moveToCenter_Action = new MoveToPoint_Action(null, self.GetComponent<NavMeshAgent>(), center.ToArray(), 0.5f);
//        wander_Action = new Wander_Action(null, self.GetComponent<NavMeshAgent>(), 7.5f);

        
//        //Composites
//        parallel_1 = new Behavior_Composite(Behavior_Composite.CompositeType.Parallel, new Behavior_Node[] { goCrazy_Action,spinningBowl_Action }, this, new Behavior_Condition[] { isInRange_Condition });

//        sequence_2 = new Behavior_Composite(Behavior_Composite.CompositeType.Sequence, new Behavior_Node[] { wander_Action, aimPlayers_Action }, this, null);
//        sequence_1 = new Behavior_Composite(Behavior_Composite.CompositeType.Sequence, new Behavior_Node[] {sequence_2, sequence_2, sequence_2, explodeAround_Action, parallel_1, fireAround_Action }, this, null);
//        root = new Behavior_Composite(Behavior_Composite.CompositeType.Selector, new Behavior_Node[] {crazyExplodeAround_Action, sequence_1, wait_Action}, this, null);
//    }
//}
