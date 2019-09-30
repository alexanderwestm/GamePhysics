using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChecker : MonoBehaviour
{
    public GameObject test1, test2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(test1.GetComponent<CollisionHull2D>().TestCollision(test2.GetComponent<CollisionHull2D>()));
    }
}
