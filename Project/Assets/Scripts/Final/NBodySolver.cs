using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct PhysicsData
{
    public Vector3 positionA;
    public Vector3 positionB;
    public Vector3 velocityA;
    public Vector3 velocityB;
    public Vector2 mass;
};

public class NBodySolver : MonoBehaviour
{
    private static NBodySolver _instance;
    public static NBodySolver Instance { get { return _instance; } }
    //private const float G = 6.67408f;
    private const float G = 6.5f;
    List<NParticle> particles;

    public ComputeShader gravityComputeShader;

    PhysicsData tempData;
    PhysicsData[] physicsDataBuffer;
    Vector3[] netForceBuffer;
    int bufferIndexer = 0;

    bool isInit = false;
    private void Awake()
    {
        particles = new List<NParticle>(FindObjectsOfType<NParticle>());
        if(particles.Count != 0)
        {
            isInit = true;
            physicsDataBuffer = new PhysicsData[particles.Count];
            netForceBuffer = new Vector3[particles.Count];
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
                for(int k = j + 1; k < particles.Count; ++k)
                {
                    // this might slow things down so do I need to actually do this?
                    // might be faster just to ship it to a computer shader
                    NParticle particleJ = particles[j], particleK = particles[k];
                    if (particles[j] != particles[k])
                    {
                        tempData = physicsDataBuffer[bufferIndexer++];
                        tempData.positionA = particleJ.position;
                        tempData.positionB = particleK.position;
                        tempData.velocityA = particleJ.velocity;
                        tempData.velocityB = particleK.velocity;
                        tempData.mass.x = particleJ.mass;
                        tempData.mass.y = particleK.mass;
                        //SolveGravity(particles[j], particles[k]);
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
        physicsDataBuffer = new PhysicsData[particles.Count];
        netForceBuffer = new Vector3[particles.Count];
        isInit = true;
    }

    private void RunComputeShader()
    {
        // 14 floats * 4 bytes (per float) = 56 bytes of data
        ComputeBuffer buffer = new ComputeBuffer(physicsDataBuffer.Length, 56);
        buffer.SetData(physicsDataBuffer);
        int kernel = gravityComputeShader.FindKernel("CSMain");
        gravityComputeShader.SetBuffer(kernel, "physicsDataBuffer", buffer);
        gravityComputeShader.Dispatch(kernel, physicsDataBuffer.Length, 1, 1);
    }
}
