using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABB : CollisionHull2D
{
    public Vector2 minBounds { get; private set; }
    public Vector2 maxBounds { get; private set; }

    public AABB(Vector2 min, Vector2 max) : base(CollisionHullType2D.AABB)
    {
        minBounds = min;
        maxBounds = max;
    }

    protected override bool TestCollisionVsCircle(Circle other)
    {
        // see circle
        return false;
    }

    protected override bool TestCollisionVsAABB(AABB other)
    {
        // if for all axes, max entent of A is greater than mix extent of B
        // 1. max-min compare: maxBounds > other.minBounds
        // 2. min-max compare: minBounds < other.maxBounds
        // 3. max && min-max

        bool max_min, min_max;
        max_min = maxBounds.x > other.minBounds.x && maxBounds.y > other.minBounds.y;
        min_max = minBounds.x < other.maxBounds.x && minBounds.y < other.maxBounds.y;
        return max_min && min_max;
    }

    protected override bool TestCollisionVsOBB(OBB other)
    {
        // same as above twice
        // find max extents of OBB, do ABB vs this box
        // then, transform this box into OBB's space, find max extents, repeat
        return false;
    }
}
