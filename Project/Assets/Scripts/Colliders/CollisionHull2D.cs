using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public CollisionHullType2D type { get; protected set; }
    protected Particle2D particle { get; set; }

    protected CollisionHull2D(CollisionHullType2D collisionType)
    {
        type = type;
    }

    public bool TestCollision(CollisionHull2D a, CollisionHull2D b)
    {
        return false;
    }

    private void Start()
    {
        particle = GetComponent<Particle2D>();
    }

    private void Update()
    {
        
    }
}
