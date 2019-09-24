using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : CollisionHull2D
{
    public float radius;
    public Circle() : base(CollisionHullType2D.CIRCLE)
    {

    }

    private bool TestCollisionVsCircle(Circle other)
    {
        Vector2 distance = other.particle.position - particle.position;
        float radiusSum = radius + other.radius;
        return Vector3.Dot(distance, distance) <= radiusSum * radiusSum;
    }

    private bool TestCollisionVsAABB(AABB other)
    {
        // find closest point to the circle on the box: clamp the (center - min) and (center - max)
        // circle point collision test using closest point
        return false;
    }

    private bool TestCollisionVsOBB(OBB other)
    {
        // same as aabb
        // multiply circle center by box world matrix inverse
        return false;
    }
}
