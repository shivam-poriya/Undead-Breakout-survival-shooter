using UnityEngine;

public class area : MonoBehaviour
{

    [SerializeField] int numberOfZombies;

    [SerializeField] GameObject originalZombie;

    [SerializeField] GameObject[] zombies;

    [SerializeField] Transform[] spawnPoints;

    [HideInInspector]public bool hasPlayerEnteredInArea;

    zombie[] zombieScripts;

    bool enteredOnce = false;

    private void Start()
    {
        zombies = new GameObject[numberOfZombies];
        zombieScripts = new zombie[numberOfZombies];   
    }

    private void Update()
    {
        if(hasPlayerEnteredInArea)
        {    
            hasPlayerEnteredInArea = false;
            spawnZombie();
            enteredOnce = true;
        }
        if(enteredOnce)
        {
            spawnZombie();
        }
    }

    void spawnZombie()
    {
        for(int i=0;i<numberOfZombies;i++)
        {
            if (zombies[i] == null)
            {
                zombies[i] = Instantiate(originalZombie, spawnPoints[i].position,Quaternion.identity,this.gameObject.transform);
                zombieScripts[i] = zombies[i].GetComponent<zombie>();
                if(enteredOnce)
                {
                    zombieScripts[i].hasFoundPlayer = true;
                    zombieScripts[i].hasScreamed = true;
                }               
            }
        }
    }
}