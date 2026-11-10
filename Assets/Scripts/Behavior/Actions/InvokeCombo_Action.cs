using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InvokeCombo_Action : Behavior_Node
{
    GameObject owner;
    SyncAnimator animator;
    float animationLength;
    float timeCounter;
    string animationToInvoke;
    Enemy script;

    public InvokeCombo_Action(Behavior_Condition[] conditions, GameObject owner, string animationToInvoke, float animationLength) : base(conditions)
    {
        this.owner = owner;
        this.script = owner.GetComponent<Enemy>();
        this.animator = owner.GetComponent<SyncAnimator>();
        this.animationLength = animationLength;
        this.animationToInvoke = animationToInvoke;
    }

    public override void Tick(float deltaTime)
    {
        if (interupted)
            return;

        timeCounter += deltaTime;
        if (timeCounter > animationLength && !interupted)
        {
            animator.SetBoolClientRPC("FightMode", false);
            FinishAction(true);
        }

    }

    override public void ExecuteAction(Behavior_Composite parentComposite)
    {
        script.ComboCount++;
        animator.SetBoolClientRPC("FightMode", true);
        base.ExecuteAction(parentComposite);
        animator.SetTriggerClientRPC(animationToInvoke);
        timeCounter = 0;
    }

    public override void Interupt()
    {
        script.ResetAllTriggers();
        animator.SetBoolClientRPC("FightMode", false);
        base.Interupt();
    }
}
