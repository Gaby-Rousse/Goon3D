using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
//enorme pisse
//public class Die : Behavior_Node
//{
//    float secondsToWait;
//    float timeCounter;
//    GameObject self;
//    SyncAnimator syncAnimator;
//    NetworkObject networkObject;
//    Enemy script;
//    public Die(Behavior_Condition[] conditions, GameObject self, float secondsToWait) : base(conditions)
//    {
//        this.self = self;
//        this.script = self.GetComponent<Enemy>();
//        this.syncAnimator = self.GetComponent<SyncAnimator>();
//        this.networkObject = self.GetComponent<NetworkObject>();
//        this.secondsToWait = secondsToWait;
//    }

//    public override void Tick(float deltaTime)
//    {
//        timeCounter += deltaTime;
//        if (timeCounter > secondsToWait)
//        {
//            if (script.IsSpawned)
//            {
//                if (script.IsServer)
//                {
//                    networkObject.Despawn();
//                    FinishAction(true);
//                }
//            }
           
//        }
//    }

//    override public void ExecuteAction(Behavior_Composite parentComposite)
//    {
//        syncAnimator.SetTriggerClientRPC("death");
//        timeCounter = 0;
//        base.ExecuteAction(parentComposite);
//    }

//    public override void Interupt()
//    {
//        //nope :)
//        return; 
//        //base.Interupt();
//    }
//}

