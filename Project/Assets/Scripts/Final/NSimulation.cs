using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NSimulation : MonoBehaviour
{
    [SerializeField] int numObjects;
    [SerializeField] float maxMass;
    [SerializeField] Vector3 lowerRangeVelocity;
    [SerializeField] Vector3 upperRangeVelocity;
    [SerializeField] Vector3 minPos;
    [SerializeField] Vector3 maxPos;

    [SerializeField] GameObject nParticlePrefab;
    private void Start()
    {
        GameObject list = GameObject.Find("ParticleList");
        for(int i = numObjects; i >= 0; --i)
        {
            GameObject obj = Instantiate(nParticlePrefab, list.transform);
            obj.GetComponent<NParticle>().Init(maxMass, lowerRangeVelocity, upperRangeVelocity, minPos, maxPos);
            obj.AddComponent<NSphereCollider>();
        }

        NBodySolver.Instance.Init();
        NCollisionChecker.Instance.Init();
    }
}
