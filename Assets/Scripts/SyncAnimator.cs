using Unity.Netcode;
using UnityEngine;

//Le network animator, j'ai probablement manqué un boutte, il marche pas pantoute
//Je vous présente le SyncAnimator. Il invoque un ClientRPC à chaque SET fait pour l'animator, tout le monde voit tout donc.
public class SyncAnimator : NetworkBehaviour
{
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();    
    }

    [ClientRpc]
    public void SetBoolClientRPC(string name, bool value)
    {
        animator.SetBool(name,value);
    }
   
    [ClientRpc]
    public void SetTriggerClientRPC(string name)
    {
        animator.SetTrigger(name);
    }

    [ClientRpc]
    public void SetFloatClientRPC(string name, float value)
    {
        animator.SetFloat(name, value);
    }

    [ClientRpc]
    public void SetIntegerClientRPC(string name, int value)
    {
        animator.SetInteger(name, value);
    }

    [ClientRpc] 
    public void ResetTriggerClientRPC(string trigger)
    {
        animator.ResetTrigger(trigger);
    }

}