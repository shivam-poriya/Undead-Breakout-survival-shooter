using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] LayerMask layerMask;
    [SerializeField] Transform weaponHolder;


    private void Start()
    {
        weaponHolder = GameObject.Find("WeaponHolder").GetComponent<Transform>();
    }

    void Update()
    {
        if(Physics.CheckSphere(transform.position, radius, layerMask))
        {
            foreach(Transform weapon in weaponHolder)
            {
                if (weapon.gameObject.activeSelf)
                {
                    pickupAmmo(weapon);
                }
            }
        }
    }

    void pickupAmmo(Transform weapon)
    {
        Rifle rifleGun = weapon.GetComponent<Rifle>();
        Pistol pistolGun = weapon.GetComponent<Pistol>();
        if (pistolGun != null)
        {
            pistolAmmo(pistolGun);   
        }
        else if (rifleGun != null)
        {
            rifleAmmo(rifleGun);   
        }
    }

    void pistolAmmo(Pistol pistolGun)
    {
        if (pistolGun.numberOfMagazines <= 10)
        {
            pistolGun.numberOfMagazines += 1;
            pistolGun.magazineCounterUI.text = "" + pistolGun.numberOfMagazines;
            Destroy(gameObject);
        }
    }

    void rifleAmmo(Rifle rifleGun)
    {
        if (rifleGun.numberOfMagazines <= 5)
        {
            rifleGun.numberOfMagazines += 1;
            rifleGun.magazineCounterUI.text = "" + rifleGun.numberOfMagazines;
            Destroy(gameObject);
        }
    }
}
