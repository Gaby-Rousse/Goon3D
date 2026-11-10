using System.Collections.Generic;
using Assets;
using UnityEditor.Build;
using UnityEngine;

//Invocateur de membre 
//Essaie d'invoquer la hitbox du membre correspondant.
public class InvokeLimb : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    ILimbOwner limbOwner;
    [SerializeField] string limbToInvoke;
    [SerializeField] bool resetHitBoxes = true;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        limbOwner = animator.GetComponent<ILimbOwner>();

        //Ceci passe par le serveur avec deux validation anti-triche.
        //1) La hitbox se désactive assurément après un certain temps
        //2) Le membre ne peut être archi spammer (genre 1 bajillions d'attaque par secondes.)
        //Un certain cooldown est mis en place, il fit le spam classique d'un joueur de beat'em'up
        //3) Seul le owner roule ceci (voir Player.cs)
       
        limbOwner.InvokeLimb(limbToInvoke, resetHitBoxes);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //Désactive le membre après utilisation
    //Présentement ResetHitBoxes fait en masse la job.
    //pas sur moé
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    if (limbOwner.GetType() == typeof(Enemy))
    //    {
    //        Enemy enemy = (Enemy)limbOwner;
    //        if (enemy.IsServer)
    //        {
    //            enemy.DisableLimbClientRpc(limbToInvoke);
    //        }

    //    }
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
