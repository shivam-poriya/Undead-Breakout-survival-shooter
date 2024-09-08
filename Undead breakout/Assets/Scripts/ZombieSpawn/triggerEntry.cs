//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class triggerEntry : MonoBehaviour
{
    [SerializeField] area spawnArea;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.transform.tag.Equals("Player"))
        {
            spawnArea.hasPlayerEnteredInArea = true;
        }
    }
}
