using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.EventSystems;
using UnityEditor.Build;
using System.Runtime.ExceptionServices;
using System.Linq;

public class HurtZone : NetworkBehaviour
{
    //Mixamo et Rigify
    string[] hands = { "mixamorig:LeftHand", "mixamorig:RightHand", "hand.L", "hand.R" };
    string[] feets = { "mixamorig:LeftToeBase", "mixamorig:RightToeBase", "toe.L", "toe.R" };
    string[] knees = { "shin.L", "shin.R" };
    //C'est quoi le nom de la hurtBox?
    [SerializeField] string hurtBox = "";
    //Quel tag seras pris en considération dans le onTriggerEnter (sinon on peux se tapper soi même!)
    public string tagToListenTo = "AttackLimb";
    //Pour invoquer le bout de son script (peu importe que se soit un joueur ou un ennemi)
    IDamageable damageable;

    void Start()
    {
        if (hurtBox == "")
        {
            hurtBox = gameObject.name;
        }
        damageable = GetComponentInParent<IDamageable>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsServer)
        {
            return;
        }
        if (other.CompareTag(tagToListenTo))
        {
            float damage = default;
            string name = other.name;
            //coup de poing = 5 dégât
            if (hands.Contains(name))
            {
                damage = 5f;
            }
            //coup de pieds = 7.5 dégât
            else if (feets.Contains(name))
            {
                damage = 7.5f;
            }
            //coup de genoux (pas mal lent) = 8.5 dégât
            else if (knees.Contains(name))
            {
                damage = 8.5f;
            }
            other.GetComponent<Collider>().enabled = false;
            //TakeDamage va se charger de calculer s'il s'agit d'un coup à la tête ou au corps.
            damageable.TakeDamage(damage, hurtBox);
        }
    }
}
