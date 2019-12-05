using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NParticle : MonoBehaviour
{
    [SerializeField] bool allowFreeMovement = false;

    public float mass;
    float massInv;
    public Vector3 position;
    public Vector3 velocity;
    [SerializeField] Vector3 acceleration;
    public Vector3 netForce;

    public bool isInit = false;

    private void Awake()
    {
        if(mass != 0)
        {
            massInv = 1f / mass;
        }
        position = transform.position;
    }

    private void Update()
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

            if (ParticleTimer.Instance.update)
            {
                UpdateKinematic(ParticleTimer.Instance.timer);
                UpdateAcceleration();
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

    private void OnDestroy()
    {
        NBodySolver.Instance.RemoveItem(this);
    }
}
