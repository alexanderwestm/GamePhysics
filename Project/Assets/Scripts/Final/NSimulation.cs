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
        NBodySolver.Instance.Init();
        NCollisionChecker.Instance.Init();
    }

    private void Update()
    {
        if(NBodySolver.Instance != null && !NBodySolver.Instance.isInit)
        {
            NBodySolver.Instance.Init();
        }
        if (NCollisionChecker.Instance != null && !NCollisionChecker.Instance.isInit)
        {
            NCollisionChecker.Instance.Init();
        }
    }

    [ContextMenu("Generate Particles")]
    public void GenerateObjects()
    {
        GameObject list = GameObject.Find("ParticleList");
        for (int i = numObjects - 1; i >= 0; --i)
        {
            GameObject obj = Instantiate(nParticlePrefab, list.transform);
            obj.GetComponent<NParticle>().Init(maxMass, lowerRangeVelocity, upperRangeVelocity, minPos, maxPos);
            obj.transform.position = obj.GetComponent<NParticle>().position;
            obj.AddComponent<NSphereCollider>();
        }
    }
}
