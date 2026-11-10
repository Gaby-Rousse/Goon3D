using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TheBoss_BT : Behavior_Tree
{
    //Composantes
    GameObject self;
    NavMeshAgent agent;
    Enemy script;

    InvokeCombo_Action head_Combo;
    InvokeCombo_Action body_Combo;
    InvokeCombo_Action other_Combo;
    Wait_Action wait_Action;
    Chase_Action chase_Action;
    IsInRange_Condition isInRange_Condition;
    HasVision_Condition hasVision_Condition;
    Seconds_Condition seconds_Condition;
    //DeathCondition death_Condition;
    ComboCount_Condition combo1;
    ComboCount_Condition combo2;
    ComboCount_Condition combo3;
    //Tree Composites
    Behavior_Composite parallel_1;
    Behavior_Composite parallel_2;
    Behavior_Composite parallel_3;
    //Die die;
    public override void InitializeTree()
    {
        if (!IsServer)
        {
            return;
        }


     

        self = this.gameObject;
        agent = self.GetComponent<NavMeshAgent>();
        script = self.GetComponent<Enemy>();

        chase_Action = new Chase_Action(null, agent, self, 3f);
        isInRange_Condition = new IsInRange_Condition(self, 1f, false);
        
        head_Combo = new InvokeCombo_Action(null,self,"Head Combo", 3f);
        body_Combo = new InvokeCombo_Action(null, self, "Body Combo", 3f);
        other_Combo = new InvokeCombo_Action(null, self, "Other Combo", 3f);
        wait_Action = new Wait_Action(null, 5f);

        combo1 = new ComboCount_Condition(script, 0,false);
        combo2 = new ComboCount_Condition(script, 1,false);
        combo3 = new ComboCount_Condition(script, 2,false);
        //death_Condition = new DeathCondition(script, false);
        //die = new Die(new Behavior_Condition[] {death_Condition}, self, 5f);


        //Interrupts
        //On réevalue l'arbre si le joueur est à proximité ou non.
        new Behavior_Interupt(this, new Behavior_Condition[] { isInRange_Condition });
        //On réevalue l'arbre si l'ennemi meurt TERRIBLE
        //MARCHE PAS PANTOUTE
        //PRÉFÈRAIT CREVER QUE COMPRENDRE CE QU'IL SE PASSE
        //new Behavior_Interupt(this, new Behavior_Condition[] { death_Condition });

        //Composites
        parallel_1 = new Behavior_Composite(Behavior_Composite.CompositeType.Parallel, new Behavior_Node[] { chase_Action, head_Combo }, this, new Behavior_Condition[] { isInRange_Condition, combo1 });
        parallel_2 = new Behavior_Composite(Behavior_Composite.CompositeType.Parallel, new Behavior_Node[] { chase_Action, body_Combo }, this, new Behavior_Condition[] { isInRange_Condition, combo2 });
        parallel_3 = new Behavior_Composite(Behavior_Composite.CompositeType.Parallel, new Behavior_Node[] { chase_Action, other_Combo }, this, new Behavior_Condition[] { isInRange_Condition, combo3 });
        
        root = new Behavior_Composite(Behavior_Composite.CompositeType.Selector, new Behavior_Node[] {parallel_1, parallel_2, parallel_3, chase_Action, wait_Action }, this, null);
    }
}
