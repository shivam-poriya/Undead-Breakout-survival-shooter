using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Sword : MonoBehaviour
{
    [SerializeField] Animator playerAnimator;
    [SerializeField] TwoBoneIKConstraint leftHand;
    [SerializeField] TwoBoneIKConstraint rightHand;
    [SerializeField] AnimationClip attackClip;
    [SerializeField] GameObject bulletUI;
    [SerializeField] GameObject attackPoint;
    [SerializeField] float attackRange;
    [SerializeField] LayerMask zombieLayer;
    [SerializeField] ParticleSystem bloodEffectPrefab;
    [SerializeField] ParticleSystem slashEffectPrefab;
    [SerializeField] Transform slashSpawnPoint;
    
    private void OnEnable()
    {
        playerAnimator.SetBool("twoHandWeapon", true);
        playerAnimator.SetFloat("weapon", 1f);
        leftHand.weight = 1f;
        rightHand.weight = 1f;
        bulletUI.SetActive(false);

    }
    private void OnDisable()
    {
        leftHand.weight = 0f;
        rightHand.weight = 0f;
        playerAnimator.SetBool("twoHandWeapon", false);
        playerAnimator.SetFloat("weapon", 0f);
    }


    void Start()
    {
                   
    }

    void Update()
    {
        bool hasAttacked = inputAttack();
        if (hasAttacked) 
        {
            StartCoroutine(spawnSlashEffect());
            StartCoroutine(attack()); 
        }          
    }
    IEnumerator spawnSlashEffect()
    {
        yield return new WaitForSeconds(0.23f);
        ParticleSystem slashEffect = Instantiate(slashEffectPrefab,slashSpawnPoint.transform.position,Quaternion.identity,slashSpawnPoint);
        slashEffect.transform.localEulerAngles = new Vector3 (0f,0f,69f);
        slashEffect.Play();
    }

    IEnumerator attack()
    {
        playerAnimator.SetTrigger("swordAttack");
        leftHand.weight = 0f;
        rightHand.weight = 0f;
        StartCoroutine(detectZombie());
        yield return new WaitForSeconds(attackClip.length-0.2f);
        leftHand.weight = 1f;
        rightHand.weight = 1f;
    }

    IEnumerator detectZombie()
    {
        yield return new WaitForSeconds(0.43f);
        Collider[] hitZombies = Physics.OverlapSphere(attackPoint.transform.position,attackRange,zombieLayer);
        foreach(Collider zombies in hitZombies)
        {
            zombie zombieScript = zombies.transform.GetComponent<zombie>();
            zombieScript.health -= 2.5f;
            spawnBloodEffect(zombies);
        }
    }
    void spawnBloodEffect(Collider zombies)
    {
        Vector3 hitPoint = zombies.ClosestPoint(transform.position);
        ParticleSystem bloodEffect = Instantiate(bloodEffectPrefab, hitPoint, Quaternion.identity);
        bloodEffect.Play();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(attackPoint.transform.position,attackRange);
    }


    bool inputAttack()
    {
        if (Input.GetButtonDown("Fire1")) { return true; }
        return false;
    }
}
