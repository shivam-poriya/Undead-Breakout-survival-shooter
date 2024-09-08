using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class Pistol : MonoBehaviour
{
    BaseGun baseGun = new BaseGun();
    [SerializeField] Charactercontroller playerScript;
    [SerializeField] GameObject aimCam;
    bool isAiming = false;
    [SerializeField] Camera mainCam;
    [SerializeField] Animator playerAnimator;
    [SerializeField] MultiAimConstraint aimRigPistol;
    [SerializeField] TwoBoneIKConstraint leftHand;
    [SerializeField] TwoBoneIKConstraint rightHand;
    float targetWeight;
    [SerializeField] MultiAimConstraint[] bodyAimRig;
    bool shot;
    [SerializeField] GameObject barrel;
    [SerializeField] ParticleSystem hitEffectPrefab;
    [SerializeField] LayerMask invisibleWallsLayer;
    [SerializeField] ParticleSystem bloodEffectPrefab;
    [SerializeField] ParticleSystem muzzleFlashPrefab;
    [SerializeField] int magazines;
    [SerializeField] int maxBullets;
    [SerializeField] TextMeshProUGUI bulletCounterUI;
    [SerializeField] public TextMeshProUGUI magazineCounterUI;
    [SerializeField] GameObject bulletUI;

    [HideInInspector] public int numberOfMagazines=-10;
    [HideInInspector] public int numberOfBullets=-10;
    WeaponChanger weaponChanger;

    private void OnDisable()
    {
        bulletUI.SetActive(false);
    }

    private void OnEnable()
    {
        bulletUI.SetActive(true);
        playerAnimator.SetBool("twoHandWeapon", false);
        weaponChanger = GameObject.Find("WeaponHolder").GetComponent<WeaponChanger>();
        bulletCounterUI.gameObject.SetActive(true);
        magazineCounterUI.gameObject.SetActive(true);
        bulletUISetup();
    }

    void bulletUISetup()
    {  
        if (numberOfMagazines < 0)
        {
            bulletCounterUI.text = "" + maxBullets;
            magazineCounterUI.text = "" + magazines;
        }
        else
        {
            bulletCounterUI.text = "" + numberOfBullets;
            magazineCounterUI.text = "" + numberOfMagazines;
        }
    }

    private void Start()
    {
        numberOfMagazines= magazines;
        numberOfBullets = maxBullets;
    }

    void Update()
    {
        setRigWeight();
        isAiming = baseGun.inputAim(playerScript,aimCam);
        setAimAnimation();  
        if(isAiming) 
        { 
            shot = inputFire();
            weaponChanger.canChange = false;
        }
        else { weaponChanger.canChange= true; }
        if (shot) { shoot(); }
        checkReload();
    }

    bool inputFire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if(numberOfBullets > 0)
            {
                return true;
            }
        }
        return false;
    }

    void shoot()
    {
        RaycastHit hit;
        hit = baseGun.shoot(mainCam, barrel, 100f,invisibleWallsLayer);
        playMuzzleFlash();
        numberOfBullets--;
        bulletCounterUI.text = "" + numberOfBullets;
        if (hit.transform!=null)
        {            
            bulletHit(hit);   
        }
    } 

    void bulletHit(RaycastHit hit)
    {
        if (hit.transform.tag.Equals("zombie"))
        {
            hitZombie(hit);
        }
        else
        {
            hitOther(hit);
        }
    }

    void hitZombie(RaycastHit hit)
    {
        zombie zombieScript = hit.transform.GetComponent<zombie>();
        zombieScript.health -= 0.5f;
        Quaternion hitRotation = Quaternion.LookRotation(hit.normal);
        ParticleSystem bloodEffect = Instantiate(bloodEffectPrefab,hit.point,hitRotation,hit.transform);
        bloodEffect.Play();
    }

    void hitOther(RaycastHit hit)
    {
        ParticleSystem hitEffect = Instantiate(hitEffectPrefab, hit.point, Quaternion.identity);
        hitEffect.Play();
        
    }
    
    void playMuzzleFlash()
    {        
        ParticleSystem muzzleFlash = Instantiate(muzzleFlashPrefab,barrel.transform.position, barrel.transform.rotation,barrel.transform);
        muzzleFlash.Play();
    }

    void setRigWeight()
    {
        aimRigPistol.weight = Mathf.Lerp(aimRigPistol.weight, targetWeight, Time.deltaTime * 10f);
        leftHand.weight = Mathf.Lerp(leftHand.weight,targetWeight,Time.deltaTime * 10f);
        rightHand.weight = Mathf.Lerp(rightHand.weight, targetWeight, Time.deltaTime * 10f);
        foreach(MultiAimConstraint bodyRig in bodyAimRig)
        {
            bodyRig.weight = Mathf.Lerp(bodyRig.weight, targetWeight, Time.deltaTime * 10f);
        }
    }

    void setAimAnimation()
    {
        if (isAiming)
        {
            targetWeight = 1f;
            playerAnimator.SetBool("pistolAim", true);
        }
        else
        {
            targetWeight = 0f;
            playerAnimator.SetBool("pistolAim", false);
        }
    }

    void checkReload()
    {
        if(Input.GetKey(KeyCode.R) && numberOfMagazines > 0 && numberOfBullets != maxBullets)
        {
            numberOfBullets = maxBullets;
            numberOfMagazines--;
            bulletCounterUI.text = "" + numberOfBullets;
            magazineCounterUI.text = "" + numberOfMagazines;
        }
    }
}
