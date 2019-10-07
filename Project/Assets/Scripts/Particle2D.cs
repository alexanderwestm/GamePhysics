using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Particle2D : MonoBehaviour
{
    public enum UpdateType
    {
        EULER = 0,
        KINEMATIC
    }

    public enum ForceType
    {
        NONE = -1,
        GRAVITY,
        NORMAL,
        SLIDING,
        FRICTION_STATIC,
        FRICTION_KINETIC,
        DRAG,
        SPRING,
        TORQUE
    }

    public enum InertiaBody
    {
        NONE = -1,
        CIRCLE,
        RECTANGLE,
        ROD,
        CIRCULAR_RING,
        SQUARE
    }

    public Vector2 position;
    [SerializeField] private Vector2 velocity = Vector2.zero;
    [SerializeField] private Vector2 acceleration = Vector2.zero;
    public float rotation = 0.0f;
    [SerializeField] private float angularVelocity = 0.0f;
    [SerializeField] private float angularAcceleration = 0.0f;
    [SerializeField] private float startMass;
    [SerializeField] private InertiaBody inertiaBody;
    [SerializeField] private float totalTorque = 0.0f;
    [SerializeField] private UpdateType updateType;
    [SerializeField] private ForceType forceType;
    [SerializeField] private bool simulate = false;


    [HideInInspector] private float mass;
    [HideInInspector] private float massInv;
    [HideInInspector] private Vector2 totalForce = Vector2.zero;
    [HideInInspector] private Vector2 forceOfGravity;
    [HideInInspector] private Vector2 normalForceUp;
    [HideInInspector] private Vector2 normalForce45;
    [HideInInspector] private Vector2 normalForceLeft;
    [HideInInspector] private float inertia;
    [HideInInspector] private float inertiaInv;
    [HideInInspector] private Vector2 centerOfMassLocal;
    [HideInInspector] private Vector2 centerOfMassGlobal;

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
        SetMass(startMass);
        SetMomentOfInertia(inertiaBody);
        centerOfMassLocal = new Vector2(transform.localScale.x / 2f, transform.localScale.y / 2f);
        centerOfMassGlobal = transform.position;

        forceOfGravity = ForceGenerator.GenerateForce_gravity(Vector2.up, 9.81f, mass);
        normalForceUp = ForceGenerator.GenerateForce_normal(forceOfGravity, Vector2.up);
        normalForce45 = ForceGenerator.GenerateForce_normal(forceOfGravity, new Vector2(1, 1));
        normalForceLeft = ForceGenerator.GenerateForce_normal(forceOfGravity, new Vector2(1, 0));
    }

    // Update is called once per frame
    void FixedUpdate()
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

            switch (forceType)
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
                    AddForce(ForceGenerator.GenerateForce_sliding(forceOfGravity, normalForce45));
                    break;
                }
                case ForceType.FRICTION_STATIC:
                {
                    AddForce(ForceGenerator.GenerateForce_friction_static(normalForceLeft, forceOfGravity, dirtWoodStatFricCoeff));
                    break;
                }
                case ForceType.FRICTION_KINETIC:
                {
                    AddForce(ForceGenerator.GenerateForce_friction_kinetic(normalForceUp, velocity, dirtWoodKinFricCoeff));
                    break;
                }
                case ForceType.DRAG:
                {
                    AddForce(forceOfGravity);
                    AddForce(ForceGenerator.GenerateForce_drag(velocity, new Vector2(0, 0), airFluidDensity, 1, cubeDragCoeff));
                    break;
                }
                case ForceType.SPRING:
                {
                    AddForce(ForceGenerator.GenerateForce_spring(position, new Vector2(0, 100), 5, .2f));
                    break;
                }
                case ForceType.TORQUE:
                {
                    AddTorque(new Vector2(1, 1), transform.position + new Vector3(.9f, 0, 0));
                    break;
                }
            }
            UpdateAcceleration();
            UpdateAngularAcceleration();
        }
        else
        {
            position = transform.position;
            rotation = transform.eulerAngles.z;   
        }
    }

    private void SetMomentOfInertia(InertiaBody body)
    {
        BoxCollider hitbox;
        Vector3 hitboxSize = Vector3.zero;
        gameObject.TryGetComponent<BoxCollider>(out hitbox);
        if(hitbox != null)
        {
            hitboxSize = hitbox.bounds.size;
        }
        switch(body)
        {
            case InertiaBody.RECTANGLE:
            {
                float dx = hitboxSize.x, dy = hitboxSize.y;
                inertia = 1f / 12f * mass * (dx * dx + dy * dy);
                break;
            }
            case InertiaBody.CIRCLE:
            {
                float radius = hitboxSize.x;
                inertia = .5f * mass * radius * radius;
                break;
            }
            case InertiaBody.ROD:
            {
                float length = hitboxSize.x;
                inertia = 1 / 12f * mass * length * length;
                break;
            }
            case InertiaBody.CIRCULAR_RING:
            {
                float outerRadius = hitboxSize.x;
                float innerRadius = outerRadius * .75f;
                float t = outerRadius - innerRadius;
                float r = t / 2;

                inertia = Mathf.PI * r * r * r * t;

                break;
            }
            case InertiaBody.SQUARE:
            {
                float dim = hitboxSize.x;
                inertia = (1f / 3f) * mass * dim * dim;
                break;
            }
        }
        inertiaInv = 1 / inertia;
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

    public void AddTorque(Vector2 force, Vector2 pointApplied)
    {
        // Formula: https://forum.unity.com/threads/how-to-calculate-how-much-torque-will-rigidbody-addforceatposition-add.287164/#post-1927110
        Vector2 relativePoint = pointApplied - centerOfMassGlobal;
        float torqueToApply = relativePoint.x * force.y - relativePoint.y * force.x;
        //float angle = Mathf.Atan2(relativePoint.y, relativePoint.x) - Mathf.Atan2(force.y, force.y);
        //float torqueToApply = relativePoint.magnitude * force.magnitude * Mathf.Sin(angle) * Mathf.Rad2Deg;
        totalTorque += torqueToApply;
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

    private void UpdateAngularAcceleration()
    {
        angularAcceleration = totalTorque * inertiaInv;
        totalTorque = 0;
    }
}
