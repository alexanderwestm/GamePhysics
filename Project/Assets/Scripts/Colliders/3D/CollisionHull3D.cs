using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CollisionHull3D : MonoBehaviour
{
    public class Collision
    {
        public struct Contact
        {
            public Vector3 point;
            public Vector3 normal;
            public float restitution;
            public float penetrationDepth;
        }

        public CollisionHull3D a = null, b = null;
        public Contact[] contact = new Contact[4];
        public int contactCount = 0;
        public bool status = false;
        
        // 7.1.1
        // if this value is negative: closing in on one another, getting closer
        // if this value is positive: moving away from one another, getting farther
        public float closingVelocity;
    }


    public enum CollisionHullType3D
    {
        INVALID_TYPE = -1,
        SPHERE,
        AABB,
        OBB,
        NUM_TYPES
    }

    public CollisionHullType3D type = CollisionHullType3D.INVALID_TYPE;// { get; protected set; }
    public Particle3D particle { get; private set; }

    protected CollisionHull3D(CollisionHullType3D collisionType)
    {
        particle = GetComponent<Particle3D>();
        type = collisionType;
    }

    private void Awake()
    {
        particle = GetComponent<Particle3D>();
    }

    // return a collision instead of a bool
    // or use ref to pass a collision
    public static bool TestCollision(CollisionHull3D a, CollisionHull3D b, out Collision collision)
    {
        switch (b.type)
        {
            case CollisionHullType3D.CIRCLE:
            {
                return a.TestCollisionVsCircle((Sphere)b, out collision);
            }
            case CollisionHullType3D.AABB:
            {
                return a.TestCollisionVsAABB((AABB)b, out collision);
            }
            case CollisionHullType3D.OBB:
            {
                return a.TestCollisionVsOBB((OBB)b, out collision);
            }
        }
        collision = null;
        return false;
    }

    protected abstract bool TestCollisionVsCircle(Sphere other, out Collision collision);
    protected abstract bool TestCollisionVsAABB(AABB other, out Collision collision);
    protected abstract bool TestCollisionVsOBB(OBB other, out Collision collision);
}
