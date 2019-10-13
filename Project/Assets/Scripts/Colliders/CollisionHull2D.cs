using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CollisionHull2D : MonoBehaviour
{
    public class Collision
    {
        public struct Contact
        {
            public Vector2 point;
            public Vector2 normal;
            public float restitution;
            public float penetrationDepth;
        }

        public CollisionHull2D a = null, b = null;
        public Contact[] contact = new Contact[4];
        public int contactCount = 0;
        public bool status = false;
        
        // 7.1.1
        // if this value is negative: closing in on one another, getting closer
        // if this value is positive: moving away from one another, getting farther
        public float closingVelocity;
    }


    public enum CollisionHullType2D
    {
        INVALID_TYPE = -1,
        CIRCLE,
        AABB,
        OBB,
        NUM_TYPES
    }

    public CollisionHullType2D type = CollisionHullType2D.INVALID_TYPE;// { get; protected set; }
    public Particle2D particle { get; private set; }

    protected CollisionHull2D(CollisionHullType2D collisionType)
    {
        particle = GetComponent<Particle2D>();
        type = collisionType;
    }

    private void Awake()
    {
        particle = GetComponent<Particle2D>();
    }

    // return a collision instead of a bool
    // or use ref to pass a collision
    public static bool TestCollision(CollisionHull2D a, CollisionHull2D b, out Collision collision)
    {
        switch (b.type)
        {
            case CollisionHullType2D.CIRCLE:
            {
                return a.TestCollisionVsCircle((Circle)b, out collision);
            }
            case CollisionHullType2D.AABB:
            {
                return a.TestCollisionVsAABB((AABB)b, out collision);
            }
            case CollisionHullType2D.OBB:
            {
                return a.TestCollisionVsOBB((OBB)b, out collision);
            }
        }
        collision = null;
        return false;
    }

    protected abstract bool TestCollisionVsCircle(Circle other, out Collision collision);
    protected abstract bool TestCollisionVsAABB(AABB other, out Collision collision);
    protected abstract bool TestCollisionVsOBB(OBB other, out Collision collision);
}
