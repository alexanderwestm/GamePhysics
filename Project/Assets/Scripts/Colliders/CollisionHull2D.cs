using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CollisionHull2D : MonoBehaviour
{
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

    public bool TestCollision(CollisionHull2D other)
    {
        switch (other.type)
        {
            case CollisionHullType2D.CIRCLE:
            {
                return TestCollisionVsCircle((Circle)other);
            }
            case CollisionHullType2D.AABB:
            {
                return TestCollisionVsAABB((AABB)other);
            }
            case CollisionHullType2D.OBB:
            {
                return TestCollisionVsOBB((OBB)other);
            }
        }
        return false;
    }

    protected abstract bool TestCollisionVsCircle(Circle other);
    protected abstract bool TestCollisionVsAABB(AABB other);
    protected abstract bool TestCollisionVsOBB(OBB other);
}
