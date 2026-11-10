using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Assets;
using MidiPlayerTK;
using MPTKDemoCatchMusic;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;
public class Player : NetworkBehaviour, IDamageable, ILimbOwner
{
    [SerializeField] float speed = 50;
    [SerializeField] float forceJump = 5;
    [SerializeField] float vitesseCamera = 1;
    [SerializeField] float gravite;
    [SerializeField] float dashSpeed = 24f;
    [SerializeField] GameObject midiPrefab;
    HealthBar healthBar;
    public NetworkVariable<float> health = new NetworkVariable<float>(100);

    SyncAnimator syncAnimator;


    //Toggle pour investigations
    [SerializeField] bool debugClosestEnemy = false;

    Vector3 jump = new Vector3(0, 0, 0);
    Animator animator;
    CharacterController charController;
    Camera playerCamera;
    float currentSpeed;
    public bool rotateTowardClosestEnemy = false;
    public bool magnetTowardClosestEnemy = false;
    GameObject closestEnemy;

    //Gestion de la musique
    MidiFilePlayer midiFilePlayer;
    MusicView musicView;
    AudioSource audioSource;

    bool isStunned = false;

    // Valeur Inputs
    Vector2 move = Vector2.zero;
    float attack = 0f;

    //Variable de temps pour les functions "timeSinceLast[...]"
    float lastPunch;
    float lastStrafe;
    float lastDash;
    bool isStrafing = false;
    float lastDeath;

    //Cooldown limb
    float lastLimbActivation = 0f;
    float limbActivationCooldown = 0.1f;

    //publique, l'animateur auras le contrôle des membres. Chaque animations auras un script contenant le nom du membre à activé.
    //L'animateur demande au serveur d'invoquer le membre en question
    public Dictionary<string, Collider> limbs = new Dictionary<string, Collider>();
    public Dictionary<string, HurtZone> hurtBoxes = new Dictionary<string, HurtZone>();

    public bool isDead = false;
    string lastAction = "revival";

    float timeSinceLastPunch()
    {
        return Time.time - lastPunch;
    }

    float timeSinceLastStrafe()
    {
        return Time.time - lastStrafe;
    }

    float timeSinceLastDash()
    {
        return Time.time - lastDash;
    }

    float timeSinceLastDeath()
    {
        return Time.time - lastDeath;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Time.timeScale = 0.5f;
        lastPunch = Time.time;
        charController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        musicView = GetComponentInChildren<MusicView>();
        midiFilePlayer = GetComponentInChildren<MidiFilePlayer>();
        audioSource = GetComponentInChildren<AudioSource>();
        if (!IsOwner)
        {
            midiPrefab.SetActive(false);
            playerCamera.GetComponentInChildren<AudioListener>().enabled = false;
            playerCamera.enabled = false;
            GetComponent<PlayerInput>().enabled = false;
            GetComponentInChildren<CinemachineCamera>().enabled = false;


            //  [Netcode] NetworkBehaviour index 2 was out of bounds for Misako(Clone).NetworkBehaviours must be the same, and in the same order, between server and client.
            //Logique après tout. Plus explicite que ça tu va droit en enfer.
            //Destroy(this);
        }
        else
        {
            
            health.OnValueChanged += UpdateHealthUI;
        }
            //https://discussions.unity.com/t/how-to-get-list-of-child-game-objects/34696/6
            //(Provient de DodgeBowls)
            //Ici, je reconstruit mon squelette, je récupère les membres clés comportant tous des hitbox
            //Pieds, Mains, Genoux.
            //Réactive l'hitbox lors d'une attaque
            //Désactive ensuite
            //La structure est celle du plugin Blender Rigify.
            Transform[] ts = GetComponentsInChildren<Transform>();
        string[] limbsToRetrieve = { "hand.L", "hand.R", "toe.L", "toe.R", "shin.L", "shin.R" };
        string[] hurtBoxesToRetrieve = { "spine.006", "torso.B", "torso.F", "torso.L", "torso.R" };
        foreach (Transform child in ts)
        {
            if (limbsToRetrieve.Contains(child.name))
            {
                limbs.Add(child.name, child.gameObject.GetComponent<Collider>());
            }
            if (hurtBoxesToRetrieve.Contains(child.name))
            {
                hurtBoxes.Add(child.name, child.gameObject.GetComponent<HurtZone>());
            }
        }

        //au départ nos membres ont les collisions désactivé. elles seront réactivé lorsque l'animateur va l'invoquer.
        ResetHitBoxes();

        if(IsServer)
        {
            syncAnimator = GetComponent<SyncAnimator>();
        }


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
        if (!IsOwner)
            return;
        if(timeSinceLastDeath() > 3.0f)
        {
            Movement();
            ClosestEnemy(transform.position, 5f);

        }

        if(isDead)
        {
            lastDeath = Time.time;
        }


        if (isDead && lastAction != "death")
        {
            lastAction = "death";
            foreach (KeyValuePair<string, HurtZone> hurtbox in hurtBoxes)
            {
                hurtbox.Value.tagToListenTo = "AttackLimb";
            }
            animator.SetTrigger("Death");
            animator.SetBool("IsDead", true);
        }
        else if(health.Value >= 100f && lastAction != "revival")
        {
            isDead = false;
            animator.SetBool("IsDead", false);
            animator.SetTrigger("Revival");
            lastAction = "revival";
            foreach (KeyValuePair<string, HurtZone> hurtbox in hurtBoxes)
            {
                hurtbox.Value.tagToListenTo = "EnemyLimb";
            }
        }

            animator.SetBool("LockedEnemy", closestEnemy != null);

        if (debugClosestEnemy)
        {
            if (closestEnemy != null)
                print(closestEnemy.name);
        }
    }

    //Lorsque en strafe, le joueur est vérouillé sur l'ennemi et ça rotation reflète cela
    void RotateTowardEnemy()
    {
        Vector3 directionToEnemy = (closestEnemy.transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);
        Quaternion rotationY = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

        transform.rotation = rotationY;
    }

    //Incapable de le faire marcher pantoute, j'abandonne
    //
    //void MagnetTowardEnemy()
    //{
    //    if(closestEnemy == null)
    //    { return; }
    //    float step = 20 * Time.deltaTime;
    //    float distanceEnemy = Vector3.Distance(closestEnemy.transform.position, transform.position);
    //    if(distanceEnemy < 1.3f)
    //    {
    //        transform.position = Vector3.MoveTowards(transform.position, closestEnemy.transform.position, step);
    //        RotateTowardEnemy();
    //        if (distanceEnemy < 0.5f)
    //        {
    //            magnetTowardClosestEnemy = false;
    //        }
    //    }
    //}

    void ClosestEnemy(Vector3 center, float radius)
    {
        //Unity l'utilise pour une explosion. Dans le cas de l'explosions, ce sont tout les colliders affecté dans l'explosion.
        //Ici, j'identifie les ennemis dans mon rayons et lequel est le plus proche.
        //On utiliseras ceci pour que le poing soit "AIMANTÉ" vers l'adversaire le plus proche (on assume que le joueur attaque toujours le plus proche, qui est devant soi. Pas de retour douteux en arrière)
        //Btw unity parle d'une version sans allocation? Effrayant.
        //https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Physics.OverlapSphere.html
        //https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Physics.OverlapSphereNonAlloc.html
        //Does not attempt to grow the buffer if it runs out of space. The length of the buffer is returned when the buffer is full. Like Physics.OverlapSphere, but generates no garbage. Additional resources: Physics.AllLayers.
        //je vais.. l'essayer . . .

        float closestValue = float.MaxValue;
        Collider closestCollider = null;
        int maxColliders = 25;
       
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(center, radius, hitColliders);
        //float maxAngle = 360f; //obselete

        foreach (Collider hitCollider in hitColliders)
        {
            //ici l'array est 25! Il risque d'y avoir des entrée vide. Poursuivons.
            if (hitCollider == null) continue;
            if (hitCollider.CompareTag("Enemy"))
            {
                Vector3 directionToTarget = hitCollider.transform.position - transform.position;

                directionToTarget.Normalize();

                // Validation de l'angle de vue (l'humble bout de code de HasVision de Martin, thank you) (sert pu a rien. . .)
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
                //if (angleToTarget < maxAngle / 2f)
               // {
                    float distance = Vector3.Distance(hitCollider.transform.position, center);
                    if (distance < closestValue)
                    {
                        closestCollider = hitCollider;
                        closestValue = distance;
                    }
               // }
            }
        }
        if (closestCollider != null)
        {
            //Pas de nouvel cible si on en vise une, si on la perd ben on la retire, pas grave.
            if(!isStrafing || closestEnemy == null)
            closestEnemy = closestCollider.gameObject;
        }
        else
        {
            closestEnemy = null;
        }

    }

    void Movement()
    {
        animator.SetBool("Strafing", rotateTowardClosestEnemy);
        if (rotateTowardClosestEnemy && closestEnemy != null)
        {
            RotateTowardEnemy();
        }
            Vector3 direction = playerCamera.transform.forward * move.y + playerCamera.transform.right * move.x;
        //float multiplyer = 1.0f;
        if (move.magnitude > 0)
        {
            //Vu que le blend animation existe, je prends juste la force du "joystick" pour savoir a quel point j'accélère.
            //N'alterne plus entre sprint et marche
            //multiplyer = Mathf.Abs(move.y) + Mathf.Abs(move.x) > 0.9 ? 3f : 1.0f;
            //print($"move = {Mathf.Abs(move.y) + Mathf.Abs(move.x)} running: {(Mathf.Abs(move.y) + Mathf.Abs(move.x) > 0.9)}");
            direction = new Vector3(direction.x, 0, direction.z);
            if (!rotateTowardClosestEnemy || closestEnemy == null)
            {
                //if(timeSinceLastDash() > 0.3f)
                transform.rotation = Quaternion.LookRotation(direction);
            }

            direction = direction.normalized;
        }
        // Vérifie si le joueur essaie de sauter et valide qu'il est présentement sur le sol 
        if (!charController.isGrounded)
        {
            jump -= transform.up * Time.deltaTime * gravite;
            currentSpeed = speed / 1.5f;
        }
        else
        {
            currentSpeed = speed;
            jump.y = Mathf.Max(-1, jump.y); // Si touche au sol, la valeur devrait être la plus haute entre -1 et Jump, pour que lorsque l'on saute pas, une force nous pousse vers le sol, mais lorsque l'on saute, la valeur soit gardé
        }

        if (rotateTowardClosestEnemy || isStunned)
        {
            currentSpeed = currentSpeed / 3f;
        }

        //Remplacé par blending tree (inspiré semaine 13 KJ1)
        //animator.SetBool("Moving", move.magnitude > 0);
        //animator.SetBool("Running", Mathf.Abs(move.y) + Mathf.Abs(move.x) > 0.9);
        //animator.SetBool("Falling", !charController.isGrounded);
        animator.SetFloat("Speed", (Mathf.Abs(move.y) + Mathf.Abs(move.x)) * 5);
        animator.SetBool("Grounded", charController.isGrounded);

        if (attack > 0)
        {
            animator.SetFloat("Punch", attack);

        }
        else
        {
            animator.SetFloat("Punch", 0);

        }


        if (timeSinceLastPunch() > 0.5f)
        {
            attack = 0;
        }

        if (isStrafing)
        {
            lastStrafe = Time.time;
        }
        if (timeSinceLastStrafe() > 0.3f)
        {
            rotateTowardClosestEnemy = false;
        }

        // Déplace le joueur selon tout les forces et Inputs
       
        if (timeSinceLastDash() < 0.05f)
        {
            charController.Move((direction * ((Mathf.Abs(move.y) + Mathf.Abs(move.x)) * dashSpeed) + jump) * Time.deltaTime);
        }
        else
        {
            charController.Move((direction * ((Mathf.Abs(move.y) + Mathf.Abs(move.x)) * currentSpeed) + jump) * Time.deltaTime);
        }
    
    }


    // Inputs

    public void InputJump(InputAction.CallbackContext context)
    {
        if (isStunned)
        {
            return;
            //nope :(
        }
        if (charController.isGrounded && !isStrafing)
        {
            jump = transform.up * forceJump;
        }
        if (isStrafing && timeSinceLastDash() > 1f)
        {
            jump = transform.up * 4f;
            animator.SetTrigger("Dodge");
            lastDash = Time.time;
        }

    }

    public void InputPunch(InputAction.CallbackContext context)
    {
        if (isStunned)
        {
            return;
            //nope :(
        }
        lastPunch = Time.time;
        if (context.performed)
        {
            attack++;
        }



    }

    public void InputKick(InputAction.CallbackContext context)
    {
        if (isStunned)
        {
            return;
            //nope :(
        }
        lastPunch = Time.time;
        if (context.started)
        {

            animator.SetTrigger("Kick");
        }
    }

    public void InputMove(InputAction.CallbackContext movement)
    {
        move = movement.ReadValue<Vector2>();
    }

    //public void InputLook(InputAction.CallbackContext movement)
    //{
    //    look = movement.ReadValue<Vector2>();
    //}

    //public void InputSprint(InputAction.CallbackContext movement)
    //{
    //    sprint = movement.ReadValue<float>();
    //}

    public void InputStrafe(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isStrafing = true; 
            rotateTowardClosestEnemy = true;
        }
        else if (context.canceled)
        {
            isStrafing = false;
        }
    }

    public void InputMenu(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            musicView.showGUI = !musicView.showGUI;
            Cursor.lockState = musicView.showGUI ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    public void InputLeftShoulder(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (musicView.showGUI)
            {
                midiFilePlayer.MPTK_Previous();
            }
        }
    }

    public void InputRightShoulder(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (musicView.showGUI)
            {
                midiFilePlayer.MPTK_Next();
            }
        }
    }

 

    //Similaire à javascript, SetInterval() qui retourne un ID
    //Voici le problème vieux comme le monde (uh)
    //Bon, je me fait taper une fois
    //isStunned = true :)
    //2 seconde je vais me rétablir (elle est faite forte tkt)
    //Mais tout à coup, un autre coup de poing atteint mon visage après 1 seconde
    //Je devrais être stun 2 autres secondes
    //Première couroutine fini 1 seconde plus tôt
    //Je ne suis plus stun de mon deuxième coup, une seconde trop tôt.
    //Tuons cette coroutine
    private Coroutine stunCoroutine;
    public void TakeDamage(float damage, string hurtBox)
    {
        if (!IsServer) return;
        print(hurtBox);
        //hardcode douteux menoum menoum :3c
        float delay = 1.0f;
        if(isDead)
        {
            health.Value = Mathf.Clamp(health.Value + 25f, 0, 100);
            return;
        }    
        if(stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }
        if (hurtBox == "head")
            damage = damage * 1.5f;
        health.Value = Mathf.Clamp(health.Value - damage, 0, 100);
        if (health.Value <= 0.0f)
        {
            isDead = true;
            return;
        }
        stunCoroutine = StartCoroutine(Stunned(delay));
        ResetHitBoxes();
        attack = 0;
        syncAnimator.SetTriggerClientRPC(hurtBox);
    }

    public void InvokeLimb(string limbName, bool resetHitBoxes)
    {
        if (IsOwner)
        {
            ActivateLimbServerRpc(limbName, resetHitBoxes);
        }
    }

    //Seul le serveur invoque l'hitbox en question pour le joueur.
    [ServerRpc]
    void ActivateLimbServerRpc(string limbName, bool resetHitBoxes)
    {
        //anti attaque infini
        if (Time.time - lastLimbActivation < limbActivationCooldown)
            return;
        ActivateLimbClientRpc(limbName, resetHitBoxes);
        lastLimbActivation = Time.time;
    }

    //On réplique le changement
    [ClientRpc]
    void ActivateLimbClientRpc(string limbName, bool resetHitBoxes)
    {
        float delay = 0.3f;
        //On désactive toutes les hitboxes
        if(resetHitBoxes)
        ResetHitBoxes();
      
        Collider limb;
        if (limbs.TryGetValue(limbName, out limb))
        {
            limb.enabled = true;
            if (limbName == "toe.L")
                delay = 0.8f;
            StartCoroutine(DisableLimbAfterDelay(limbName, delay));
        }
    }

    IEnumerator DisableLimbAfterDelay(string limbName, float delay)
    {
        yield return new WaitForSeconds(delay);
        Collider limb;
        if (limbs.TryGetValue(limbName, out limb))
            limb.enabled = false;
    }

    IEnumerator Stunned(float delay)
    {
        attack = 0;
        isStunned = true;
        yield return new WaitForSeconds(delay);
        isStunned = false;
        //FINI naturellement
        stunCoroutine = null;
    }

    public void UpdateHealthUI(float oldValue, float newValue)
    {
        if (!IsOwner) return;
        if (healthBar == null)
            healthBar = FindFirstObjectByType<HealthBar>();
        healthBar.UpdateHealthPercent(Mathf.Clamp(newValue,0,100), oldValue < newValue);
    }
}