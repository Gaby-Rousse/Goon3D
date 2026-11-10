using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : NetworkBehaviour, IDamageable, ILimbOwner
{
    //L'ennemi est très similaire au Player, allez voir Player.cs pour plus de commentaires
    Animator animator;
    SyncAnimator syncAnimator;
    NavMeshAgent navMeshAgent;
    //public GameObject closest;
    public Dictionary<string, Collider> limbs = new Dictionary<string, Collider>();
    CharacterController characterController;
    int comboCount = 0;
    public int ComboCount { get { return comboCount % 3; } set { comboCount = value; } }
    [SerializeField] float health = 1f;
    public bool IsDead { get { return health <= 0f; } }

    float lastHitTime = 0f;
    public int hitCount = 0;
    float timeSinceLastHit()
    {
        return Time.time - lastHitTime;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        syncAnimator = GetComponent<SyncAnimator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        Transform[] ts = GetComponentsInChildren<Transform>();
        //des beau nom <3 
        string[] limbsToRetrieve = { "mixamorig:LeftToeBase", "mixamorig:RightToeBase", "mixamorig:LeftHand", "mixamorig:RightHand" };
        foreach (Transform child in ts)
        {
            if (limbsToRetrieve.Contains(child.name))
            {
                limbs.Add(child.name, child.gameObject.GetComponent<Collider>());
            }
        }

        //au départ nos membres ont les collisions désactivé. elles seront réactivé lorsque l'animateur va l'invoquer.
        ResetHitBoxes();

        characterController = GetComponent<CharacterController>();
    }

    public void ResetHitBoxes()
    {
        //https://stackoverflow.com/questions/141088/how-to-iterate-over-a-dictionary
        foreach (KeyValuePair<string, Collider> limb in limbs)
        {
            limb.Value.enabled = false;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (IsDead)
            return;
        if (IsServer)
        {
            syncAnimator.SetFloatClientRPC("Speed", navMeshAgent.velocity.magnitude);
            if (timeSinceLastHit() > 1f)
                hitCount = 0;
        }
    }

    Coroutine deathCoroutine;
    public void TakeDamage(float damage, string hurtBox)
    {
        if (!IsServer) return;
        print("HurtBox touched = " + hurtBox);
        print("Raw Damage dealt = " + damage);
        if (hurtBox == "head")
        {
            damage = damage * 1.5f;
        }
        print("Damage dealt = " + damage);
        health -= damage;
        print("Health = " + health);

        ResetHitBoxes();
        if (IsDead)
        {
            if(deathCoroutine == null)
            deathCoroutine = StartCoroutine(Death(3f));
            return;
        }
        hitCount++;
        lastHitTime = Time.time;
        syncAnimator.SetTriggerClientRPC(hurtBox);
    }

    public void InvokeLimb(string limbName, bool resetHitBoxes)
    {
        if (IsServer)
        {
            ActivateLimbClientRpc(limbName, resetHitBoxes);
        }
    }

    //On réplique le changement
    [ClientRpc]
    void ActivateLimbClientRpc(string limbName, bool resetHitBoxes)
    {
        if (resetHitBoxes)
            ResetHitBoxes();

        Collider limb;
        if (limbs.TryGetValue(limbName, out limb))
        {
            limb.enabled = true;
        }

        StartCoroutine(DisableLimbAfterDelay(limbName, 1f));
    }

    [ClientRpc]
    public void DisableLimbClientRpc(string limbName)
    {
        Collider limb;
        if (limbs.TryGetValue(limbName, out limb))
        {
            limb.enabled = true;
        }
    }

    //PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE PENIBLE 
    IEnumerator Death(float delay)
    {
        //TUER L'ARBRE UNE BONNE FOIS POUR TOUTE
        //VA T'EN!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        Behavior_Tree tree = GetComponent<Behavior_Tree>();
        if (tree != null)
        {
            //ong décoliss pour vrai sivousplait
            tree.StopTree();
            Destroy(tree);
        }

        //reset les animations
        ResetAllTriggers();

        //go death
        syncAnimator.SetTriggerClientRPC("Death");
        syncAnimator.SetBoolClientRPC("IsDying",true);


        //attendre le temps de l'animation
        yield return new WaitForSeconds(delay);

        //Despawn l'objet ensuite. bye . . .
        if (IsSpawned)
        {
            GetComponent<NetworkObject>().Despawn();
        }

        //OK
        //DÉSOLÉ POUR LE BORDEL
        //VOICI LES EXPLICATIONS:
        //La mort de l'ennemi marchait pas pantoute. Il se faisait tuer et continuait de donner des coups poing, puis ensuite je l'ai mis dans un behavior condition
        //pour interrompre l'arbre mais le isRange forcait une réevaluation lui aussi donc je sais pas ça boguait pas mal
        //ensuite j'ai mis la mort en coroutine sur le script de l'enemi, quand il détecte sa mort il lance la coroutine ci dessus
        //mais ça marchait toujours pas, l'ennemi donnait encore des coups de poing donc j'ai amenée des changements tous autant plus douteux que l'autre car je mettais
        //la faute sur la behavior tree, je sais toujours pas si c'est d'ailleur le fautif
        //mais les modifications sont les suivantes
        //l'ajout d'une fonction stopTree(), qui retire les nodes ou le node actif et met la root à null
        //Le update regarde si la root est null, si elle est null return; en boucle
        //Car l'object est destroy à un moment douteux que j'ai pas trop saisi, le script plante car le navAgent n'existe plus
        //Mais is inRange essaie de l'utiliser, donc dans Interrupt, j'ai mis un try catch, qui attrape toutes les exception et les ignore (pas super secure)
        //Ça parait tu que je fait du php ?
        //Blague à part, ça pas régler le problème
        //Du moins partiellement, l'ennemi ne tappe plus!!!!!!!! mais il tente encore de courir
        //Donc maintenant c'est un mix de IsDead et Death trigger, ben bordélique mais il ne peut pas frapper s'il est mort
        //Il ne peut pas courir s'il est mort, etc bon
        //Conclusion, ou je veux en venir, dans le fond c'est un gros bordel monstre de genre 67 solution scotché
        //Mais je sais pas c'est laquelle exactement qui as fait fonctionner le tout, donc je laisse tout, le jeu fonctionne dorénavant.
    }


    IEnumerator DisableLimbAfterDelay(string limbName, float delay)
    {
        yield return new WaitForSeconds(delay);
        Collider limb;
        if (limbs.TryGetValue(limbName, out limb))
            limb.enabled = false;
    }

    //reset les triggers
    //https://discussions.unity.com/t/reset-all-animationtriggers/812262
    public void ResetAllTriggers()
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                syncAnimator.ResetTriggerClientRPC(param.name);
            }
        }
    }
}
