using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Rifle : MonoBehaviour
{
    [HideInInspector] public bool beingUnequipped;
    BaseGun baseGun = new BaseGun();
    [SerializeField] Animator playerAnimator;
    [SerializeField] TwoBoneIKConstraint LeftHandIdle;
    [SerializeField] TwoBoneIKConstraint leftHand;
    [SerializeField] TwoBoneIKConstraint rightHand;
    bool isAiming = false;
    [SerializeField] Charactercontroller playerScript;
    [SerializeField] GameObject aimCam;
    float targetWeight;
    [SerializeField] MultiAimConstraint aimRigRifle;
    [SerializeField] MultiAimConstraint[] bodyAimRig;
    bool shot;
    WeaponChanger weaponChanger;
    [SerializeField] Camera mainCam;
    [SerializeField] GameObject barrel;
    [SerializeField] LayerMask invisibleWallsLayer;
    [SerializeField] ParticleSystem bloodEffectPrefab;
    [SerializeField] ParticleSystem hitEffectPrefab;
    [SerializeField] ParticleSystem muzzleFlashPrefab;
    [SerializeField] int magazines;
    [SerializeField] int maxBullets;
    [SerializeField] TextMeshProUGUI bulletCounterUI;
    [SerializeField] public TextMeshProUGUI magazineCounterUI;
    bool isReloading=false;
    [SerializeField] AnimationClip reloadAniClip;
    [SerializeField] GameObject bulletUI;


    [HideInInspector] public int numberOfMagazines = -10;
    [HideInInspector] public int numberOfBullets = -10;
    private void OnDisable()
    {
        bulletUI.SetActive(false);
        LeftHandIdle.weight = 0f;
        targetWeight = 0f;
        playerAnimator.SetBool("twoHandWeapon", false);
        setRigWeight();
    }

    private void OnEnable()
    {
        bulletUI.SetActive(true);
        playerAnimator.SetFloat("weapon", 0f);
        playerAnimator.SetBool("twoHandWeapon", true);
        weaponChanger = GameObject.Find("WeaponHolder").GetComponent<WeaponChanger>();
        LeftHandIdle.weight = Mathf.Lerp(leftHand.weight, 1f, Time.deltaTime * 10f);
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
    void Start()
    {
        numberOfMagazines = magazines;
        numberOfBullets = maxBullets;
    }
    
    
    void Update()
    {
        if (isReloading == false)
        {
            setRigWeight();
            if (!isReloading) { isAiming = baseGun.inputAim(playerScript, aimCam); }
            setAimAnimation();
            if (isAiming)
            {
                shot = inputFire();
                weaponChanger.canChange = false;
            }
            else { if (!isReloading) { weaponChanger.canChange = true; } }
            if (shot) { shoot(); }
            checkReload();
        }
    }
    void setAimAnimation()
    {
        if (isAiming)
        {
            LeftHandIdle.weight = 0f;
            targetWeight = 1f;
            playerAnimator.SetBool("rifleAim", true);
        }
        else
        {
            LeftHandIdle.weight = 1f;
            targetWeight = 0f;
            playerAnimator.SetBool("rifleAim", false);
        }
    }

    void setRigWeight()
    {
        aimRigRifle.weight = Mathf.Lerp(aimRigRifle.weight, targetWeight, Time.deltaTime * 10f);
        leftHand.weight = Mathf.Lerp(leftHand.weight, targetWeight, Time.deltaTime * 10f);
        rightHand.weight = Mathf.Lerp(rightHand.weight, targetWeight, Time.deltaTime * 10f);
        foreach (MultiAimConstraint bodyRig in bodyAimRig)
        {
            bodyRig.weight = Mathf.Lerp(bodyRig.weight, targetWeight, Time.deltaTime * 10f);
        }
    }
    bool inputFire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (numberOfBullets > 0)
            {
                return true;
            }
        }
        return false;
    }
    void shoot()
    {
        RaycastHit hit;
        hit = baseGun.shoot(mainCam, barrel, 100f, invisibleWallsLayer);
        playMuzzleFlash();
        numberOfBullets--;
        bulletCounterUI.text = "" + numberOfBullets;
        if (hit.transform != null)
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
        zombieScript.health -= 1f;
        Quaternion hitRotation = Quaternion.LookRotation(hit.normal);
        ParticleSystem bloodEffect = Instantiate(bloodEffectPrefab, hit.point, hitRotation, hit.transform);
        bloodEffect.Play();
    }

    void hitOther(RaycastHit hit)
    {
        ParticleSystem hitEffect = Instantiate(hitEffectPrefab, hit.point, Quaternion.identity);
        hitEffect.Play();

    }
    void playMuzzleFlash()
    {
        ParticleSystem muzzleFlash = Instantiate(muzzleFlashPrefab, barrel.transform.position, barrel.transform.rotation, barrel.transform);
        muzzleFlash.Play();
    }
    void checkReload()
    {
        if (Input.GetKey(KeyCode.R) && numberOfMagazines > 0 && numberOfBullets != maxBullets && !isReloading)
        {
            StartCoroutine(waitForReload());
            
        }
    }

    IEnumerator waitForReload()
    {
        isReloading = true;
        isAiming = false;
        playerScript.aim = false;
        aimCam.SetActive(false);
        setAimAnimation();
        LeftHandIdle.weight = 0f;
        targetWeight = 0f;
        setRigWeight();
        playerAnimator.SetBool("rifleAim",false);
        playerAnimator.SetBool("Reload",true);
        
        resetWeight();
        
        yield return new WaitForSeconds(2.3f);

        playerAnimator.SetBool("Reload", false);
        numberOfBullets = maxBullets;
        numberOfMagazines--;
        bulletCounterUI.text = "" + numberOfBullets;
        magazineCounterUI.text = "" + numberOfMagazines;
        isReloading = false;
        LeftHandIdle.weight = 1f;
    }

    void resetWeight()
    {
        aimRigRifle.weight = 0f;
        leftHand.weight = 0f;
        rightHand.weight = 0f;
        foreach (MultiAimConstraint bodyRig in bodyAimRig)
        {
            bodyRig.weight = 0f;
        }
    }
}