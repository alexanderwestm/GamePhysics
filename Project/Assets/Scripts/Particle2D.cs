using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle2D : MonoBehaviour
{
    enum UpdateType
    {
        EULER = 0,
        KINEMATIC
    }

    enum ForceType
    {
        NONE = -1,
        GRAVITY,
        NORMAL,
        SLIDING,
        FRICTION_STATIC,
        FRICTION_KINETIC,
        DRAG,
        SPRING
    }

    public Vector2 position { get; private set; }
    [SerializeField] Vector2 velocity;
    [SerializeField] Vector2 acceleration;
    [SerializeField] float rotation;
    [SerializeField] float angularVelocity;
    [SerializeField] float angularAcceleration;
    [SerializeField] float startMass;
    [SerializeField] UpdateType updateType;
    [SerializeField] ForceType forceType;
    [SerializeField] bool simulate;

    private float mass, massInv;
    private Vector2 totalForce;
    private Vector2 forceOfGravity, normalForceUp, normalForce45, normalForceLeft;


    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
        SetMass(startMass);
        forceOfGravity = ForceGenerator.GenerateForce_gravity(Vector2.up, 9.81f, mass);
        normalForceUp = ForceGenerator.GenerateForce_normal(forceOfGravity, Vector2.up);
        normalForce45 = ForceGenerator.GenerateForce_normal(forceOfGravity, new Vector2(1, 1));
        normalForceLeft = ForceGenerator.GenerateForce_normal(forceOfGravity, new Vector2(1, 0));
    }

    // Update is called once per frame
    void Update()
    {
        //https://www.khanacademy.org/science/ap-physics-1/ap-forces-newtons-laws/friction-ap/v/static-and-kinetic-friction-example
        float dirtWoodStatFricCoeff = .6f, dirtWoodKinFricCoeff = .55f;
        float cubeDragCoeff = 1.05f, airFluidDensity = .001225f;
        if (simulate)
        {
            transform.position = position;
            transform.rotation = Quaternion.Euler(0, 0, rotation);

            switch (updateType)
            {
                case UpdateType.EULER:
                {
                    UpdatePositionEulerExplicit(Time.deltaTime);
                    UpdateRotationEulerExplicit(Time.deltaTime);
                    break;
                }
                case UpdateType.KINEMATIC:
                {
                    UpdatePositionKinematic(Time.deltaTime);
                    UpdateRotationKinematic(Time.deltaTime);
                    break;
                }
            }

            switch(forceType)
            {
                case ForceType.GRAVITY:
                {
                    AddForce(forceOfGravity);
                    break;
                }
                case ForceType.NORMAL:
                {
                    AddForce(forceOfGravity);
                    AddForce(normalForceUp);
                    break;
                }
                case ForceType.SLIDING:
                {
                    break;
                }
                case ForceType.FRICTION_STATIC:
                {
                    break;
                }
                case ForceType.FRICTION_KINETIC:
                {
                    break;
                }
                case ForceType.DRAG:
                {
                    break;
                }
                case ForceType.SPRING:
                {
                    break;
                }
            }
        }
    }

    public void SetMass(float newMass)
    {
        mass = newMass > 0 ? newMass : 0;
        massInv = mass > 0 ? 1 / mass : 0;
    }

    public float GetMass()
    {
        return mass;
    }

    public void AddForce(Vector2 force)
    {
        totalForce += force;
    }

    private void UpdatePositionEulerExplicit(float dt)
    {
        position += velocity * dt;
        velocity += acceleration * dt;
    }

    private void UpdatePositionKinematic(float dt)
    {
        position += velocity * dt + .5f * acceleration * dt * dt;
        velocity += acceleration * dt;
    }

    private void UpdateRotationEulerExplicit(float dt)
    {
        rotation += angularVelocity * dt;
        angularVelocity += angularAcceleration * dt;
    }

    private void UpdateRotationKinematic(float dt)
    {
        rotation += angularVelocity * dt + .5f * angularAcceleration * dt * dt;
        angularVelocity += angularAcceleration * dt;
    }

    private void UpdateAcceleration()
    {
        acceleration = massInv * totalForce;
        totalForce = Vector2.zero;
    }
}
