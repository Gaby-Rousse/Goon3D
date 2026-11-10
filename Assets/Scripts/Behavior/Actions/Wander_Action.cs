using UnityEngine;
using UnityEngine.AI;

public class Wander_Action : Behavior_Node
{
    NavMeshAgent agent;
    float range;

    public Wander_Action(Behavior_Condition[] behavior_Conditions, NavMeshAgent agent, float range) : base(behavior_Conditions)
    {
        this.agent = agent;
        this.range = range;
    }

    public override void ExecuteAction(Behavior_Composite parent_composite)
    {
        agent.SetDestination(new Vector3(agent.transform.position.x + Random.Range(-range, range), agent.transform.position.y, agent.transform.position.z + Random.Range(-range, range)));
        base.ExecuteAction(parent_composite);
    }

    public override void Tick(float deltaTime)
    {
        if (agent.remainingDistance <= agent.stoppingDistance + .5f)
            FinishAction(true);
        base.Tick(deltaTime);
    }
}
