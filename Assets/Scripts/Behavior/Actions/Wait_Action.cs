using UnityEngine;

public class Wait_Action : Behavior_Node
{
    float secondsToWait;
    float timeCounter;
    public Wait_Action(Behavior_Condition[] conditions, float secondsToWait) : base(conditions)
    {
        this.secondsToWait = secondsToWait;
    }

    public override void Tick(float deltaTime)
    {
        if (interupted)
            return;

        timeCounter += deltaTime;
        if (timeCounter > secondsToWait && !interupted)
        {
            FinishAction(true);
        }

    }

    override public void ExecuteAction(Behavior_Composite parentComposite)
    {
        timeCounter = 0;
        base.ExecuteAction(parentComposite);
    }
}

