using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestObject : MonoBehaviour
{

    [SerializeField] private GameObject[] AllObjects;
    [SerializeField] private GameObject NearestObject;
    float distance;

    float nearestDistance = 1000;
    // Start is called before the first frame update
    void Start()
    {
        AllObjects = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < AllObjects.Length; i++)
        {
            distance = Vector3.Distance(this.transform.position, AllObjects[i].transform.position);

            if (distance < nearestDistance)
            {
                NearestObject = AllObjects[i];
                nearestDistance = distance;
            }
        }
    }
}
