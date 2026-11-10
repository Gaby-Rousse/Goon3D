using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class MoveToPoint_Action : Behavior_Node
{
    NavMeshAgent agent;
    Transform[] targets;
    float stopDistance;
    int index;

    public MoveToPoint_Action(Behavior_Condition[] conditions, NavMeshAgent agent, Transform[] targets, float stopDistance) : base(conditions)
    {
        this.agent = agent;
        this.targets = targets;
        this.stopDistance = stopDistance;

        index = 0;
    }

    public override void Tick(float deltaTime)
    {
        if (interupted)
            return;

        if ((agent.destination - agent.transform.position).magnitude < stopDistance + 1.5f)
        {
            agent.SetDestination((agent.transform.position - agent.destination).normalized * stopDistance + agent.destination);
            index = ++index % targets.Length;
            FinishAction(true);
        }
        else
        {
            agent.SetDestination(targets[index].position);

            FinishAction(false);
        }
    }

    override public void ExecuteAction(Behavior_Composite parentComposite)
    {
        base.ExecuteAction(parentComposite);
        agent.SetDestination(targets[index].position);
    }


    public override void Interupt()
    {
        agent.destination = agent.transform.position;
        base.Interupt();
    }
}