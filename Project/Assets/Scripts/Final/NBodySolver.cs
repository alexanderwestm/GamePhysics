using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct PhysicsData
{
    public Vector3 positionA;
    public Vector3 positionB;
    public Vector2 mass;

    public Vector3 output;

    public void SetData(NParticle a, NParticle b)
    {
        positionA = a.position;
        positionB = b.position;
        mass.x = a.mass;
        mass.y = b.mass;
        output = Vector3.zero;
    }
};

public struct ParticleData
{
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 acceleration;
    public float massInv;
    public Vector3 netForce;

    public void SetData(NParticle particle)
    {
        position = particle.position;
        velocity = particle.velocity;
        acceleration = particle.acceleration;
        netForce = particle.netForce;
        massInv = particle.massInv;
    }
};

struct InfluencePairs
{
    public NParticle a;
    public NParticle b;
};

public class NBodySolver : MonoBehaviour
{
    private static NBodySolver _instance;
    public static NBodySolver Instance { get { return _instance; } }
    //private const float G = 6.67408f;
    private const float G = 6.5f;
    List<NParticle> particles;
    List<InfluencePairs> influencePairs;

    public ComputeShader gravityComputeShader;
    ComputeBuffer gravityComputeBuffer;

    PhysicsData[] physicsDataBuffer;
    //Vector3[] netForceBuffer;
    int gravityBufferIndexer = 0;

    public ComputeShader particleUpdateComputeShader;
    ComputeBuffer particleComputeBuffer;
    ParticleData[] particleDataBuffer;

    public bool isInit = false;
    private void Awake()
    {
        NParticle[] nParticles = FindObjectsOfType<NParticle>();
        if(nParticles != null && nParticles.Length > 0)
        {
            Init();
            //particles = new List<NParticle>(FindObjectsOfType<NParticle>());
            //
            //isInit = true;
            //physicsDataBuffer = new PhysicsData[particles.Count * particles.Count];
            //influencePairs = new List<InfluencePairs>();
            //
            //particleDataBuffer = new ParticleData[particles.Count];
            //netForceBuffer = new Vector3[particles.Count];
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
            SetGravityBufferData();
            RunGravityCompute();
            SetParticleBufferData();
            RunUpdateCompute();
        }
    }

    public void RemoveItem(NParticle particle)
    {
        particles.Remove(particle);
        particleDataBuffer = new ParticleData[particles.Count];
        physicsDataBuffer = new PhysicsData[particles.Count * particles.Count];
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
        physicsDataBuffer = new PhysicsData[particles.Count * particles.Count];
        influencePairs = new List<InfluencePairs>();

        particleDataBuffer = new ParticleData[particles.Count];
        //netForceBuffer = new Vector3[particles.Count];
        isInit = true;
    }

    private void SetGravityBufferData()
    {
        NParticle particleJ, particleK;
        for (int j = 0; j < particles.Count; ++j)
        {
            particleJ = particles[j];
            for (int k = 0; k < particles.Count; ++k)
            {
                // this might slow things down so do I need to actually do this?
                // might be faster just to ship it to a computer shader
                particleK = particles[k];
                if (particles[j] != particles[k])
                {
                    physicsDataBuffer[gravityBufferIndexer].SetData(particleJ, particleK); 
                    //physicsDataBuffer[gravityBufferIndexer].positionA = particleJ.position;
                    //physicsDataBuffer[gravityBufferIndexer].positionB = particleK.position;
                    //physicsDataBuffer[gravityBufferIndexer].mass.x = particleJ.mass;
                    //physicsDataBuffer[gravityBufferIndexer].mass.y = particleK.mass;

                    gravityBufferIndexer++;

                    influencePairs.Add(new InfluencePairs() { a = particleJ, b = particleK });
                    //SolveGravity(particles[j], particles[k]);
                }
            }
        }
        gravityBufferIndexer = 0;
    }

    private void SetParticleBufferData()
    {
        for(int i = particles.Count - 1; i >= 0; --i)
        {
            particleDataBuffer[i].SetData(particles[i]);
        }
    }

    private void RunGravityCompute()
    {
        // 17 floats * 4 bytes (per float) = 68 bytes of data
        gravityComputeBuffer = new ComputeBuffer(physicsDataBuffer.Length, 44);
        gravityComputeBuffer.SetData(physicsDataBuffer);
        int kernel = gravityComputeShader.FindKernel("CSMain");
        gravityComputeShader.SetBuffer(kernel, "physicsDataBuffer", gravityComputeBuffer);
        gravityComputeShader.Dispatch(kernel, 32, 32, 1);
        gravityComputeBuffer.GetData(physicsDataBuffer);

        for(int i = influencePairs.Count - 1; i >= 0; --i)
        {
            influencePairs[i].a.netForce += physicsDataBuffer[i].output;
            influencePairs[i].b.netForce -= physicsDataBuffer[i].output;
        }
        influencePairs.Clear();
        gravityComputeBuffer.Release();
    }

    private void RunUpdateCompute()
    {
        // 13 floats * 4 bytes = 52 bytes
        particleComputeBuffer = new ComputeBuffer(particleDataBuffer.Length, 52);
        particleComputeBuffer.SetData(particleDataBuffer);
        int kernel = particleUpdateComputeShader.FindKernel("KinematicUpdate");
        particleUpdateComputeShader.SetBuffer(kernel, "particleDataBuffer", particleComputeBuffer);
        particleUpdateComputeShader.SetFloat("deltaTime", ParticleTimer.Instance.timer);
        particleUpdateComputeShader.Dispatch(kernel, 32, 32, 1);
        particleComputeBuffer.GetData(particleDataBuffer);

        for (int i = particles.Count - 1; i >= 0; --i)
        {
            particles[i].SetParticleData(particleDataBuffer[i]);
        }
        particleComputeBuffer.Release();
    }
}
