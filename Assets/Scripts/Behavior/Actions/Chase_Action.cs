using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class Chase_Action : Behavior_Node
{
    NavMeshAgent agent;
    GameObject closest;
    GameObject[] players;
    GameObject self;
    Enemy script;
    //Enemy script;
    float maxChaseTime;
    float currentChaseTime;
    public Chase_Action(Behavior_Condition[] conditions, NavMeshAgent agent, GameObject self, float maxChaseTime) : base(conditions)
    {
        this.agent = agent;
        this.maxChaseTime = maxChaseTime;
        this.self = self;
        this.script = self.GetComponent<Enemy>();
        currentChaseTime = 0;
    }

    public override void Tick(float deltaTime)
    {
        if (interupted)
        {
            currentChaseTime = 0;
            return;
        }

        currentChaseTime += deltaTime;

        if (currentChaseTime >= maxChaseTime)
        {
            currentChaseTime = 0;
            FinishAction(true);
        }
        else
            agent.SetDestination(closest.transform.position);

        //Le dos de l'ennemi est sa faiblesse, mais. . . l'ennemi punie si tu spam trop là.
        float step = (2.5f + script.hitCount) * deltaTime;
        Vector3 directionToEnemy = (closest.transform.position - agent.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);
        self.transform.rotation = Quaternion.LerpUnclamped(self.transform.rotation,targetRotation,step);
        //if (Vector3.Distance(closest.transform.position, agent.transform.position) <= 2f)
        //{
        //    agent.radius = 0.1f;
        //}

    }

    override public void ExecuteAction(Behavior_Composite parentComposite)
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        float smallestDistance = float.MaxValue;
        foreach (GameObject player in players)
        {
            if(player.GetComponent<Player>().isDead)
            {
                continue;
            }
            if (Vector3.Distance(player.transform.position, agent.transform.position) < smallestDistance)
            {
                closest = player;
                //script.closest = closest;
                smallestDistance = Vector3.Distance(player.transform.position, agent.transform.position);
            }
        }
        agent.SetDestination(closest.transform.position);
        base.ExecuteAction(parentComposite);
    }


    public override void Interupt()
    {
        agent.destination = agent.transform.position;
        currentChaseTime = 0;
        base.Interupt();
    }


}
