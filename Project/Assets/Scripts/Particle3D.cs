using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Particle3D : MonoBehaviour
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
        SPHERE,
        HOLLOW_SPHERE,
        BOX,
        CUBE,
        HOLLOW_BOX,
        HOLLOW_CUBE,
        CYLINDER,
        CONE
    }

    [Header("Mass")]
    [SerializeField] private float startMass;
    public float massInv;

    [Header("Position")]
    public Vector3 position;
    public Vector3 velocity = Vector3.zero;
    [SerializeField] private Vector3 acceleration = Vector3.zero;

    [Header("Rotation")]
    public PhysicsQuaternion rotation = PhysicsQuaternion.identity;
    [SerializeField] private Vector3 angularVelocity = Vector3.zero;
    [SerializeField] private Vector3 angularAcceleration = Vector3.zero;

    [Header("Torque")]
    [SerializeField] private InertiaBody inertiaBody;
    [SerializeField] private Vector3 totalTorque = Vector3.zero;

    [Header("Settings")]
    [SerializeField] private UpdateType updateType;
    [SerializeField] private ForceType forceType;
    [SerializeField] private bool simulate = false;


    private float mass;
    private Vector3 totalForce = Vector2.zero;
    private Vector3 forceOfGravity;

    private Matrix4x4 inertia;
    private Matrix4x4 inertiaInv;
    private Matrix4x4 worldInertia;
    private Matrix4x4 worldInertiaInv;

    private Matrix4x4 worldTransformMatrix;
    private Matrix4x4 worldTransformMatrixInverse;

    public Vector3 centerOfMassLocal { get; private set; }
    public Vector3 centerOfMassGlobal { get; private set; }

    [SerializeField] float accelerationGravity;

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
        rotation = new PhysicsQuaternion(transform.rotation);
        SetMass(startMass);
        SetMomentOfInertia(inertiaBody);
        worldTransformMatrix = new Matrix4x4();
        centerOfMassLocal = new Vector3(transform.localScale.x / 2f, transform.localScale.y / 2f, transform.localScale.z / 2f);

        forceOfGravity = ForceGenerator.GenerateForce_gravity(Vector3.up, accelerationGravity, mass);
    }

    public void ReInit()
    {
        position = Vector3.zero;
        velocity = Vector3.zero;
        acceleration = Vector3.zero;
        rotation = PhysicsQuaternion.identity;
        angularVelocity = Vector3.zero;
        angularAcceleration = Vector3.zero;

        totalForce = Vector3.zero;
        totalTorque = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //https://www.khanacademy.org/science/ap-physics-1/ap-forces-newtons-laws/friction-ap/v/static-and-kinetic-friction-example
        float dirtWoodStatFricCoeff = .6f, dirtWoodKinFricCoeff = .55f;
        float cubeDragCoeff = 1.05f, airFluidDensity = .001225f;

        if (simulate)
        {
            UpdateParticleData();
            UpdateWorldMatrix();

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
                    //AddForce(normalForceUp);
                    break;
                }
                case ForceType.SLIDING:
                {
                    //AddForce(ForceGenerator.GenerateForce_sliding(forceOfGravity, normalForce45));
                    break;
                }
                case ForceType.FRICTION_STATIC:
                {
                    //AddForce(ForceGenerator.GenerateForce_friction_static(normalForceLeft, forceOfGravity, dirtWoodStatFricCoeff));
                    break;
                }
                case ForceType.FRICTION_KINETIC:
                {
                    //AddForce(ForceGenerator.GenerateForce_friction_kinetic(normalForceUp, velocity, dirtWoodKinFricCoeff));
                    break;
                }
                case ForceType.DRAG:
                {
                    AddForce(forceOfGravity);
                    AddForce(ForceGenerator.GenerateForce_drag(velocity, new Vector3(0, 0, 0), airFluidDensity, 1, cubeDragCoeff));
                    break;
                }
                case ForceType.SPRING:
                {
                    AddForce(ForceGenerator.GenerateForce_spring(position, new Vector3(0, 100, 0), 5, .2f));
                    break;
                }
                case ForceType.TORQUE:
                {
                    AddTorque(new Vector3(1, 1, 1), transform.position + new Vector3(.9f, 0, 0), false);
                    break;
                }
            }

            UpdateAcceleration();
            UpdateAngularAcceleration();
        }
        else
        {
            position = transform.position;
            rotation = new PhysicsQuaternion(transform.rotation);
        }
    }

    private void SetMomentOfInertia(InertiaBody body)
    {
        inertia = Matrix4x4.identity;
        inertiaInv = Matrix4x4.identity;
        if (InertiaBody.NONE == body) return;

        Vector3 hitboxSize = Vector3.zero;
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (meshFilter != null)
        {
            hitboxSize.x = meshFilter.mesh.bounds.size.x * transform.localScale.x;
            hitboxSize.y = meshFilter.mesh.bounds.size.y * transform.localScale.y;
            hitboxSize.z = meshFilter.mesh.bounds.size.z * transform.localScale.z;
        }
        else if (renderer != null)
        {
            hitboxSize.x = renderer.sprite.bounds.size.x * transform.localScale.x;
            hitboxSize.y = renderer.sprite.bounds.size.y * transform.localScale.y;
            hitboxSize.z = renderer.sprite.bounds.size.z * transform.localScale.z;
        }

        //hitboxSize.z = mesh.bounds.size.z * transform.localScale.z;

        switch (body)
        {
            case InertiaBody.BOX:
            {
                float oneSixth = 1 / 6;
                float side = hitboxSize.x;
                float numToSet = oneSixth * mass * side * side;
                inertia.SetRow(0, new Vector4(numToSet, 0, 0, 0));
                inertia.SetRow(1, new Vector4(0, numToSet, 0, 0));
                inertia.SetRow(2, new Vector4(0, 0, numToSet, 0));
                break;
            }
            case InertiaBody.CONE:
            {
                float radius = hitboxSize.x * .5f;
                float height = hitboxSize.y;
                float firstSecond = .6f * mass * height * height + .15f * mass * radius * radius;
                float third = .3f * mass * radius * radius;
                inertia.SetRow(0, new Vector4(firstSecond, 0, 0, 0));
                inertia.SetRow(1, new Vector4(0, firstSecond, 0, 0));
                inertia.SetRow(2, new Vector4(0, 0, third, 0));
                break;
            }
            case InertiaBody.CUBE:
            {
                float oneTwelfth = 1 / 12;
                float first = oneTwelfth * mass * (hitboxSize.y * hitboxSize.y + hitboxSize.z * hitboxSize.z);
                float second = oneTwelfth * mass * (hitboxSize.z * hitboxSize.z + hitboxSize.x * hitboxSize.x);
                float third = oneTwelfth * mass * (hitboxSize.x * hitboxSize.x + hitboxSize.y * hitboxSize.y);
                inertia.SetRow(0, new Vector4(first, 0, 0, 0));
                inertia.SetRow(1, new Vector4(0, second, 0, 0));
                inertia.SetRow(2, new Vector4(0, 0, third, 0));
                break;
            }
            case InertiaBody.CYLINDER:
            {
                float radius = hitboxSize.x * .5f;
                float height = hitboxSize.y * .5f;
                float oneTwelfth = 1 / 12;
                float firstSecond = oneTwelfth * mass * (3 * radius * radius + height * height);
                float third = .5f * mass * radius * radius;
                inertia.SetRow(0, new Vector4(firstSecond, 0, 0, 0));
                inertia.SetRow(1, new Vector4(0, firstSecond, 0, 0));
                inertia.SetRow(2, new Vector4(0, 0, third, 0));
                break;
            }
            case InertiaBody.HOLLOW_BOX:
            {
                float fiveThirds = 5 / 3;
                float first = fiveThirds * mass * (hitboxSize.y * hitboxSize.y + hitboxSize.z * hitboxSize.z);
                float second = fiveThirds * mass * (hitboxSize.z * hitboxSize.z + hitboxSize.x * hitboxSize.x);
                float third = fiveThirds * mass * (hitboxSize.x * hitboxSize.x + hitboxSize.y * hitboxSize.y);
                inertia.SetRow(0, new Vector4(first, 0, 0, 0));
                inertia.SetRow(1, new Vector4(0, second, 0, 0));
                inertia.SetRow(2, new Vector4(0, 0, third, 0));
                break;
            }
            case InertiaBody.HOLLOW_CUBE:
            {
                float fiveThirdsOverTwo = (5/3) * .5f;
                float side = hitboxSize.x;
                float numToSet = fiveThirdsOverTwo * mass * side * side;
                inertia.SetRow(0, new Vector4(numToSet, 0, 0, 0));
                inertia.SetRow(1, new Vector4(0, numToSet, 0, 0));
                inertia.SetRow(2, new Vector4(0, 0, numToSet, 0));
                break;
            }
            case InertiaBody.HOLLOW_SPHERE:
            {
                float radius = hitboxSize.x * .5f;
                // 2/3mr^2
                float numToSet = 2 / 3 * mass * radius * radius;
                inertia.SetRow(0, new Vector4(numToSet, 0, 0, 0));
                inertia.SetRow(1, new Vector4(0, numToSet, 0, 0));
                inertia.SetRow(2, new Vector4(0, 0, numToSet, 0));
                break;
            }
            case InertiaBody.SPHERE:
            {
                float radius = hitboxSize.x * .5f;
                // 2/5mr^2
                float numToSet = .4f * mass * radius * radius;
                inertia.SetRow(0, new Vector4(numToSet, 0, 0, 0));
                inertia.SetRow(1, new Vector4(0, numToSet, 0, 0));
                inertia.SetRow(2, new Vector4(0, 0, numToSet, 0));
                break;
            }
            default:
            {
                break;
            }
        }
        inertiaInv = inertia.inverse;
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

    public void AddForce(Vector3 force)
    {
        totalForce += force;
    }

    private void UpdateParticleData()
    {
        transform.position = position;
        transform.rotation = rotation.GetQuaternion();
        centerOfMassGlobal = transform.position;
    }

    private void UpdateWorldMatrix()
    {
        Matrix4x4 rotationMatrix = rotation.GetRotationMatrix();
        Vector4 positionVector = new Vector4(centerOfMassGlobal.x, centerOfMassGlobal.y, centerOfMassGlobal.z, 1);

        worldTransformMatrix.SetColumn(0, rotationMatrix.GetColumn(0));
        worldTransformMatrix.SetColumn(1, rotationMatrix.GetColumn(1));
        worldTransformMatrix.SetColumn(2, rotationMatrix.GetColumn(2));
        worldTransformMatrix.SetColumn(3, positionVector);

        worldTransformMatrixInverse.SetRow(0, rotationMatrix.GetColumn(0));
        worldTransformMatrixInverse.SetRow(1, rotationMatrix.GetColumn(1));
        worldTransformMatrixInverse.SetRow(2, rotationMatrix.GetColumn(2));
        worldTransformMatrixInverse.SetRow(3, positionVector);
    }

    public void AddTorque(Vector3 force, Vector3 pointApplied, bool local)
    {
        // Formula: https://forum.unity.com/threads/how-to-calculate-how-much-torque-will-rigidbody-addforceatposition-add.287164/#post-1927110
        Vector2 relativePoint;
        if (local)
        {
            relativePoint = pointApplied - centerOfMassLocal;
        }
        else
        {
            relativePoint = pointApplied - centerOfMassGlobal;
        }
        //float torqueToApply = relativePoint.x * force.y - relativePoint.y * force.x;
        //totalTorque += torqueToApply;
    }

    private void UpdatePositionEulerExplicit(float dt)
    {
        // x(t+dt) = x(t) + v(t)dt
        // F(t+dt) = F(t) + f(t)dt + (dF/ dt)dt
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
        PhysicsQuaternion angularVelQuat = new PhysicsQuaternion(angularVelocity.x, angularVelocity.y, angularVelocity.z, 0);
        rotation += .5f * angularVelQuat * rotation * dt;
        rotation.Normalize();

        angularVelocity += angularAcceleration * dt;
    }

    private void UpdateRotationKinematic(float dt)
    {
        //rotation += angularVelocity * dt + .5f * angularAcceleration * dt * dt;
        //rotation %= 360;
        //angularVelocity += angularAcceleration * dt;
        PhysicsQuaternion velQuat = new PhysicsQuaternion(angularVelocity.x, angularVelocity.y, angularVelocity.z, 0), accQuat = new PhysicsQuaternion(angularAcceleration.x, angularAcceleration.y, angularAcceleration.z, 0);
        //rotation += .5f * velQuat * rotation * dt + (accQuat * rotation + .5f * velQuat * velQuat * rotation) * dt * dt;
        rotation += velQuat * rotation * dt * .5f + accQuat * rotation * dt * dt * .25f - rotation * angularVelocity.sqrMagnitude * dt * dt * .125f;
        rotation.Normalize();
        angularVelocity += angularAcceleration * dt;
    }

    private void UpdateAcceleration()
    {
        acceleration = massInv * totalForce;
        totalForce = Vector2.zero;
    }

    private void UpdateAngularAcceleration()
    {
        //angularAcceleration = totalTorque * inertiaInv;
        angularAcceleration = inertiaInv.MultiplyVector(totalTorque);
        totalTorque = Vector3.zero;
    }
}
