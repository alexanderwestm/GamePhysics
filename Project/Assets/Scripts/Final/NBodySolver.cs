using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBodySolver : MonoBehaviour
{
    private static NBodySolver _instance;
    public static NBodySolver Instance { get { return _instance; } }
    //private const float G = 6.67408f;
    private const float G = 6.5f;
    List<NParticle> particles;

    bool isInit = false;
    private void Awake()
    {
        particles = new List<NParticle>(FindObjectsOfType<NParticle>());
        if(particles.Count != 0)
        {
            isInit = true;
        }
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogError("Duplicate NBodySolver");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isInit && ParticleTimer.Instance.update)
        {
            for(int j = 0; j < particles.Count; ++j)
            {
                for(int k = 0; k < particles.Count; ++k)
                {
                    // this might slow things down so do I need to actually do this?
                    // might be faster just to ship it to a computer shader
                    if (particles[j] != particles[k])
                    {
                        SolveGravity(particles[j], particles[k]);
                    }
                }
            }
        }
    }

    public void RemoveItem(NParticle particle)
    {
        particles.Remove(particle);
    }

    private void SolveGravity(NParticle a, NParticle b)
    {
        Vector3 distance = b.position - a.position;
        Vector3 addForce = G * a.mass * b.mass * distance.normalized;
        addForce /= Vector3.Dot(distance, distance);

        a.netForce += addForce;
    }

    public void Init()
    {
        particles = new List<NParticle>(FindObjectsOfType<NParticle>());
        isInit = true;
    }
}
