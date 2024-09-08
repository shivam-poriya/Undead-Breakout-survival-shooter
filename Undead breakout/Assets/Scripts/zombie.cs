using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class zombie : MonoBehaviour
{
    public Charactercontroller playerScript;
    public bool hasAttacked=false;
    public bool hasHit;
    public SkinnedMeshRenderer skinMesh;
    public SkinnedMeshRenderer skinPants;
    public SkinnedMeshRenderer skinTop;
    public float health;
    [SerializeField]
    private float speed;
    public LayerMask layerMask;
    public float radius = 2f;
    public float maxDistance = 1f;
    public GameObject player;
    public NavMeshAgent agent;
    [SerializeField]
    private float attackRange;
    public float amount;
    public GameObject body;
    public Material originalZ1;
    public Material originalZ2;
    public Material zombie1;
    public Material zombie2;
    public AnimationClip clip;
    public GameObject redHollow;
    public Transform placeSpawnEffect;
    public float waitForSeconds;
    public bool isSpawned = false;
    public ParticleSystem spawnEffect;
    public AnimationClip scream;
    public Transform zombieParent;
    public AudioSource screamSound;
    public AudioSource dieSound;
    public GameObject ammo;
    public GameObject spawnAmmo;
    public Transform ammoSpawnPlace;
    public GameObject spawnRaycast;
    [HideInInspector] public bool hasFoundPlayer = false;
    [HideInInspector] public bool hasScreamed = false;
    [HideInInspector] public bool isDead = false;
    private bool dissolve = false;
    private ParticleSystem copySpawnEffect;
    Material[] mz;
    bool isDying = false;
    float control = 0f;
    private Animator animator;
    void Start()
    {
        hasHit = false;
        health = 5f;
        zombieMaterialsINIT();
        getEssentialComponent();
        copySpawnEffect = Instantiate(spawnEffect, placeSpawnEffect.position, Quaternion.identity, placeSpawnEffect);
        copySpawnEffect.Play();
    }

    void getEssentialComponent()
    {
        zombieParent = GetComponentInParent<Transform>();
        playerScript = FindAnyObjectByType<Charactercontroller>();
        player = GameObject.Find("Player");
        animator = GetComponent<Animator>();
    }

    void zombieMaterialsINIT()
    {
        zombie1 = Instantiate(originalZ1);
        zombie2 = Instantiate(originalZ2);
        mz = new Material[2];
        mz[0] = zombie1;
        mz[1] = zombie2;
        skinMesh.materials = mz;
        skinPants.materials = mz;
        skinTop.materials = mz;
    }

    void spawnZombie()
    {
        transform.LookAt(player.transform.position);
        Vector3 rotate = new Vector3(0f, transform.eulerAngles.y, 0f);
        transform.eulerAngles = rotate;
        
    }

    void Update()
    {
        if (isSpawned){
            if (playerScript.isDeadOnGround) { goIdleAnimation(); }
            else{
                if (health <= 0f) { 
                    die(); 
                }
                else if (hasFoundPlayer && isDying == false){
                    StartCoroutine(waitForScream());
                    if (isDying == false) { 
                        chasePlayer(); 
                    }
                }
                else{
                    if(agent!=null){
                        agent.isStopped = true;
                        if (!isDying) { 
                            checkForPlayer(); 
                        }
                    }
                }
            }
        }
        else { StartCoroutine(waitForSpawning()); }
    }

    void goIdleAnimation()
    {
        animator.SetBool("isRunning", false);
        animator.SetBool("attack", false);
    }

    IEnumerator waitForSpawning()
    {
        animator.SetBool("spawn", true);
        spawnZombie();
        yield return new WaitForSeconds(7f);
        isSpawned = true;
        
        
    }
    IEnumerator redGojo()
    {
        redHollow.SetActive(true);
        yield return new WaitForSeconds(waitForSeconds);
        dissolve = true;
    }

    void die()
    {
        isDying = true;
        animator.SetBool("dead",true);
        if(dieSound.isPlaying == false) { dieSound.Play(); }
        if (agent != null) { setCollider(); }
        agent = null;
        hasFoundPlayer = false;
        animator.SetBool("isRunning",false);
        animator.SetBool("attack",false);
        StartCoroutine(redGojo());
        dissolveAnimation();
    }

    void dissolveAnimation()
    {
        if (dissolve)
        {
            control += amount;
            zombie1.SetFloat("_Dissolve", control);
            zombie2.SetFloat("_Dissolve", control);
        }
        if (control >= 0.9f)
        {
            isDead = true;
            isDead = false;
            spawnAmmo = Instantiate(ammo, ammoSpawnPlace.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void setCollider()
    {
        agent.height = 0f;
        agent.radius = 0f;
        CapsuleCollider collider = GetComponent<CapsuleCollider>();
        collider.radius = 0f;
        collider.height = 0f;
        agent.isStopped = true;
    }

    IEnumerator waitForAttack()
    {
        
        
        yield return new WaitForSeconds(0.8f);
        
        animator.SetBool("attack", false);
        animator.SetBool("isRunning", false);    
        if (Physics.CheckSphere(spawnRaycast.transform.position, attackRange-0.001f, layerMask))
        {
            playerScript.isDead = true;
        }
    }

    void chasePlayer()
    {     
        //Rotates in z axis
        transform.LookAt(player.transform.position);
        //To stop the rotation of z axis
        Vector3 rotate = new Vector3(0f, transform.eulerAngles.y, 0f);
        transform.eulerAngles = rotate;
        attack();
    }

    void attack()
    {
        if (Physics.CheckSphere(transform.position, attackRange, layerMask)){
            if (playerScript.isDeadOnGround){
                agent.isStopped = true;
                animator.SetBool("isRunning", false);
            }
            else{
                animator.SetBool("attack", true);
                agent.isStopped = true;
                StartCoroutine(waitForAttack());
            }
        }
        else{
            animator.SetBool("attack", false);
            agent.isStopped = false;
            animator.SetBool("isRunning", true);
            agent.SetDestination(player.transform.position);
        }
    }

    void checkForPlayer()
    {
        
        if(Physics.CheckSphere(transform.position,radius,layerMask))
        {
            if(hasFoundPlayer == false)
            {
                transform.LookAt(player.transform.position);
                StartCoroutine(waitForScream());
            }
            
            
        }
        
    }
    IEnumerator waitForScream()
    {
        agent.isStopped = true;
        animator.SetBool("scream", true);
        if(isSpawned && hasScreamed == false){
            screamSound.Play();
            hasScreamed = true;            
        }
        yield return new WaitForSeconds(scream.length);
        if (agent != null) { agent.isStopped = false; }
        animator.SetBool("scream",false);
        hasFoundPlayer = true; 
    }
}
