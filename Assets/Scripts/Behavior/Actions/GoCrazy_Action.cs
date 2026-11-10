using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class GoCrazy_Action : Behavior_Node
{
    float dashingTime;
    float elapsedTime = 0f;
    GameObject closest;
    GameObject self;
    GameObject[] players;
    NavMeshAgent agent;
    public GoCrazy_Action(Behavior_Condition[] conditions, GameObject go, NavMeshAgent nav, float dashingTime = 10f) : base(conditions)
    {
        this.dashingTime = dashingTime;
        self = go;
        agent = nav;
    }

    //Update
    public override void Tick(float deltaTime)
    {
        if (interupted)
            return;

        self.transform.Rotate(new Vector3(0, 4000 * deltaTime, 0));

        agent.SetDestination(closest.transform.position);
        
        elapsedTime += deltaTime;

        if (elapsedTime >= dashingTime)
        {
            base.FinishAction(true);
        }
    }

    //Start
    override public void ExecuteAction(Behavior_Composite parentComposite)
    {
        elapsedTime = 0f;
        agent.autoBraking = false; 

        players = GameObject.FindGameObjectsWithTag("Player");
        float smallestDistance = 6767676767676767676767676767676767f; 
        foreach (GameObject player in players)
        {
            if(Vector3.Distance(player.transform.position, self.transform.position) < smallestDistance)
            {
                closest = player;
                smallestDistance = Vector3.Distance(player.transform.position, self.transform.position);
            }
        }
        base.ExecuteAction(parentComposite);
    }

    public override void FinishAction(bool result)
    {
        agent.autoBraking = true;
        base.FinishAction(result);
    }

    //Interrupt
    public override void Interupt()
    {
        base.Interupt();
    }
}
