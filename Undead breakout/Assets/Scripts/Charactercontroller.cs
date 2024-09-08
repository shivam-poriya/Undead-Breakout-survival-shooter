using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class Charactercontroller : MonoBehaviour
{
    public Image bloodEffect;
    public float health;
    public GameObject bulletPrefab;
    public Transform barrel;
    public float bulletHitMissDestance = 25f;
    public GameObject crosshair;
    public GameObject aimCamera;
    public CharacterController controller;
    public bool isDeadOnGround = false;
    public AudioSource walkingSound;
    public AudioSource runningSound;
    public AudioSource hitSound;
    public GameObject restartMenu;
    public Animator animator;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float dieWait;
    public bool aim;
    public bool isDead = false;
    public float runSpeed;
    public float speed = 6f;
    public float jumpHeight = 5f;
    public float turnSmoothTime = 0.1f;
    public Transform cam;
    [SerializeField] WeaponChanger weaponChangeScript;
    [SerializeField] Rig parentRig;
    /* Private variables */

    bool run;
    float gravity = -9.81f;
    float turnSmoothVelocity;
    Vector3 velocity;
    bool isGrounded;
    bool playedHitSound = false;
    float blendFloat = 0.0f;
    private void Start()
    {
        health = 5f;
        Cursor.lockState = CursorLockMode.Locked;
        animator = GetComponent<Animator>();
        restartMenu.SetActive(false);
        
    }

    void Update()
    {
        addGravity();
        if (isDead)
        {
            StartCoroutine(waitForDie());
        }
        if (!isDeadOnGround)
        {
            Vector3 direction = getMovementInput();
            changeCrosshairStatus();
            movePlayer(direction);
            changeAnimation(direction);
            applyGravity();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            restartMenu.SetActive(true);
        }
        applyGravity();
    }
    IEnumerator waitForDie()
    {
        yield return new WaitForSeconds(dieWait);
        if(hitSound.isPlaying == false && playedHitSound==false)
        {
            hitSound.Play();
            playedHitSound = true;
        }
        walkingSound.Stop();
        runningSound.Stop();
        setDyingAnimation();
        aimCamera.SetActive(false);
        isDeadOnGround = true;
        makeBloodAppear();
        
    }

    void setDyingAnimation()
    {
        animator.SetBool("twoHandWeapon", false);
        animator.SetBool("pistolAim", false);
        animator.SetBool("rifleAim", false);        
        parentRig.weight = 0f;
        if(weaponChangeScript.selectedWeapon==0||weaponChangeScript.selectedWeapon==2)
        {
            animator.SetTrigger("dieUnarmed");
        }
        else if(weaponChangeScript.selectedWeapon==1)
        {
            animator.SetTrigger("dieSword");
        }
        else
        {
            animator.SetTrigger("dieRifle");
        }
    }

    void makeBloodAppear()
    {
        while(bloodEffect.fillAmount<1f)
        {
            bloodEffect.fillAmount += 0.1f;
        }
    }

    void addGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -3f;
        }
    }

    Vector3 getMovementInput()
    {
        run = Input.GetKey(KeyCode.LeftShift);
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        return new Vector3(horizontal, 0f, vertical).normalized;
    }

    void changeCrosshairStatus()
    {
        if (aim)
        {
            crosshair.SetActive(true);
        }
        else
        {
            crosshair.SetActive(false);
        }
    }

    void movePlayer(Vector3 direction)
    {
        if (direction.magnitude >= 0.1f){
            if (!aim){
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                Vector3 moveDir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
                if (run) { controller.Move(moveDir.normalized * runSpeed * Time.deltaTime); }
                else { controller.Move(moveDir.normalized * speed * Time.deltaTime); }
            }
        }
        
    }

    void applyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void changeAnimation(Vector3 direction)
    {
        if (!aim)
        {
            if (!(direction.magnitude >= 0.1f)) { blendFloat = 0f; }

            else if (direction.magnitude >= 0.1f && !run) { blendFloat = 0.5f; }

            else if (run && direction.magnitude >= 0.1f) { blendFloat = 1f; }

            animator.SetFloat("blendBase", blendFloat, 0.1f, Time.deltaTime);
        }
        else
        {
            blendFloat = 0f;
            animator.SetFloat("blendBase", blendFloat, 0.1f, Time.deltaTime);
        }
    }

    
}