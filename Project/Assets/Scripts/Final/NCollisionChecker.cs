using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NCollisionChecker : MonoBehaviour
{
    private static NCollisionChecker _instance;
    public static NCollisionChecker Instance { get { return _instance; }}

    List<NSphereCollider> sphereColliders;
    List<NSphereCollider> toDelete;

    bool isInit = false;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogError("Duplicate NCollisionChecker");
        }
        toDelete = new List<NSphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isInit)
        {
            for(int j = sphereColliders.Count - 1; j >= 0; --j)
            {
                for(int k = j - 1; k >= 0; --k)
                {
                    NSphereCollider a = sphereColliders[j], b = sphereColliders[k];
                    if (!toDelete.Contains(a) && !toDelete.Contains(b))
                    {
                        if(a.CheckCollision(b))
                        {
                            if(a.particle.mass > b.particle.mass)
                            {
                                toDelete.Add(b);
                            }
                            else
                            {
                                toDelete.Add(a);
                            }
                            RespondToCollision(a, b);
                        }
                    }
                }
            }

            foreach(NSphereCollider col in toDelete)
            {
                Destroy(col.gameObject);
            }
            toDelete.Clear();
        }
    }

    private void RespondToCollision(NSphereCollider a, NSphereCollider b)
    {
        float aMass = a.particle.mass, bMass = b.particle.mass;
        Vector3 aVel = a.particle.velocity, bVel = b.particle.velocity;
        // this seems wrong?
        a.particle.velocity = (aMass * aVel + bMass * bVel) / (aMass + bMass);
    }

    public void RemoveItem(NSphereCollider col)
    {
        sphereColliders.Remove(col);
    }

    public void Init()
    {
        sphereColliders = new List<NSphereCollider>(FindObjectsOfType<NSphereCollider>());
        isInit = true;
    }
}
