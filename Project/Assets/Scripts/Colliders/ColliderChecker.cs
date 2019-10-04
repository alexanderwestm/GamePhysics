using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChecker : MonoBehaviour
{
    public GameObject test1, test2;
    public Material redMat, greenMat;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(CollisionHull2D.TestCollision(test1.GetComponent<CollisionHull2D>(), test2.GetComponent<CollisionHull2D>()))
        {
            test1.GetComponent<MeshRenderer>().material = redMat;
            test2.GetComponent<MeshRenderer>().material = redMat;
        }
        else
        {
            test1.GetComponent<MeshRenderer>().material = greenMat;
            test2.GetComponent<MeshRenderer>().material = greenMat;
        }
    }
}
