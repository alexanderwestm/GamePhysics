using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSystemTest : MonoBehaviour
{
    [SerializeField] Material redMaterial;
    [SerializeField] Material normalMaterial;

    List<CollisionHull3D> collisionHulls;

    // Start is called before the first frame update
    void Start()
    {
        collisionHulls = new List<CollisionHull3D>(FindObjectsOfType<CollisionHull3D>());
    }

    // Update is called once per frame
    void Update()
    {
        CollisionHull3D.Collision col;
        for(int j = 0; j < collisionHulls.Count; ++j)
        {
            for (int k = j + 1; k < collisionHulls.Count; ++k)
            {
                if (collisionHulls[j] != collisionHulls[k])
                {
                    if (CollisionHull3D.TestCollision(collisionHulls[j], collisionHulls[k], out col))
                    {
                        collisionHulls[j].gameObject.GetComponent<Renderer>().material = redMaterial;
                        collisionHulls[k].gameObject.GetComponent<Renderer>().material = redMaterial;
                    }
                    else
                    {
                        collisionHulls[j].gameObject.GetComponent<Renderer>().material = normalMaterial;
                        collisionHulls[k].gameObject.GetComponent<Renderer>().material = normalMaterial;
                    }
                }
            }
        }
    }
}
