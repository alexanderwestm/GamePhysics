using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NParticle : MonoBehaviour
{
    [SerializeField] bool allowFreeMovement = false;

    public float mass;
    public float massInv;
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 acceleration;
    public Vector3 netForce;

    public bool isInit = false;
    public int ID = -1;

    private void Awake()
    {
        if(mass != 0)
        {
            massInv = 1f / mass;
        }
        position = transform.position;
    }

    private void FixedUpdate()
    {
        if (isInit)
        {
            if (!allowFreeMovement)
            {
                transform.position = position;
            }
            else
            {
                position = transform.position;
            }

            if(!ParticleTimer.Instance.useCompute)
            {
                if (ParticleTimer.Instance.update)
                {
                    UpdateKinematic(ParticleTimer.Instance.timer);
                    UpdateAcceleration();
                }
            }
        }
    }

    private void UpdateKinematic(float dt)
    {
        position += velocity * dt + acceleration * .5f * dt * dt;
        velocity += acceleration * dt;
    }

    private void UpdateAcceleration()
    {
        acceleration = massInv * netForce;
        netForce = Vector3.zero;
    }

    public void Init(float maxMass, Vector3 lowerRangeVel, Vector3 upperRangeVel, Vector3 lowerRangePos, Vector3 upperRangePos)
    {
        mass = Random.Range(0, maxMass) * 10;
        if(mass != 0)
        {
            massInv = 1 / mass;
        }
        velocity = ExtensionMethods.RandomVector(lowerRangeVel, upperRangeVel);
        position = ExtensionMethods.RandomVector(lowerRangePos, upperRangePos);
        transform.localScale = new Vector3(mass, mass, mass);
        isInit = true;
    }

    public void SetParticleData(ParticleData data)
    {
        position = data.position;
        velocity = data.velocity;
        acceleration = data.acceleration;
        netForce = data.netForce;
    }

    private void OnDestroy()
    {
        NBodySolver.Instance.RemoveItem(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + velocity);
    }
}
