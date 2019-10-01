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
            Vector2 point;
            Vector2 normal;
            float restitution;
        }

        public CollisionHull2D a = null, b = null;
        public Contact[] contact = new Contact[4];
        public int contactCount = 0;
        public bool status = false;
        
        public Vector2 closingVelocity;
    }


    public enum CollisionHullType2D
    {
        INVALID_TYPE = -1,
        CIRCLE,
        AABB,
        OBB,
        NUM_TYPES
    }

    public CollisionHullType2D type;
    public Particle2D particle;

    protected CollisionHull2D(CollisionHullType2D collisionType)
    {
        this.type = collisionType;
    }

    private void Start()
    {
        particle = GetComponent<Particle2D>();
    }


    // return a collision instead of a bool
    // or use ref to pass a collision
    public static bool TestCollision(CollisionHull2D a, CollisionHull2D b)
    {
        switch (b.type)
        {
            case CollisionHullType2D.CIRCLE:
            {
                return a.TestCollisionVsCircle((Circle)b);
            }
            case CollisionHullType2D.AABB:
            {
                return a.TestCollisionVsAABB((AABB)b);
            }
            case CollisionHullType2D.OBB:
            {
                return a.TestCollisionVsOBB((OBB)b);
            }
        }
        return false;
    }

    protected abstract bool TestCollisionVsCircle(Circle other);
    protected abstract bool TestCollisionVsAABB(AABB other);
    protected abstract bool TestCollisionVsOBB(OBB other);
}
