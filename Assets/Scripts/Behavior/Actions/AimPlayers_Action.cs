//using Unity.VisualScripting;
//using UnityEngine;
//using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
//using static UnityEngine.UI.GridLayoutGroup;

//public class AimPlayers_Action : Behavior_Node
//{
//    int counter = 0;
//    int numberOfFire;
//    float cooldown;
//    bool canFire = false;
//    float elapsedTime = 0f;
//    GameObject self;
//    public AimPlayers_Action(Behavior_Condition[] conditions, GameObject go,int numberOfFire = 1,  float cooldown = 1f) : base(conditions)
//    {
  
//        self = go;
//        this.cooldown = cooldown;
//        this.numberOfFire = numberOfFire;
//    }

//    //Update
//    public override void Tick(float deltaTime)
//    {
//        if (interupted)
//            return;

//        if (!canFire)
//        {
//            elapsedTime += deltaTime;
//            canFire = (elapsedTime >= cooldown);
//        }


//        if (canFire)
//        {
//            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
//            foreach (GameObject player in players)
//                GameManager.Instance.invokeBossBowl(Bowl.BowlType.Classic, self.transform.position, Quaternion.LookRotation(player.transform.position - self.transform.position));
//            canFire = false;
//            elapsedTime = 0f;
//            counter++;
//        }
//        if (counter >= numberOfFire)
//        {
//            base.FinishAction(true);
//        }
//    }

//    //Start
//    override public void ExecuteAction(Behavior_Composite parentComposite)
//    {
//        counter = 0;
//        base.ExecuteAction(parentComposite);
//    }

//    //Interrupt
//    public override void Interupt()
//    {
//        base.Interupt();
//    }
//}
