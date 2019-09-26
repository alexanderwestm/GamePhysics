using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : CollisionHull2D
{
    public float radius;
    public Circle() : base(CollisionHullType2D.CIRCLE)
    {

    }

    protected override bool TestCollisionVsCircle(Circle other)
    {
        Vector2 distance = other.particle.position - particle.position;
        float radiusSum = radius + other.radius;
        return Vector3.Dot(distance, distance) <= radiusSum * radiusSum;
    }

    protected override bool TestCollisionVsAABB(AABB other)
    {
        // find closest point to the circle on the box: clamp the (center - min) and (center - max)
        // circle point collision test using closest point

        float xPart = Mathf.Clamp(particle.position.x, other.minBounds.x, other.maxBounds.x);
        float yPart = Mathf.Clamp(particle.position.y, other.minBounds.y, other.maxBounds.y);

        float deltaX = particle.position.x - xPart;
        float deltaY = particle.position.y - yPart;

        Vector2 deltaVector = new Vector2(deltaX, deltaY);
        return deltaVector.sqrMagnitude < radius * radius;
    }

    protected override bool TestCollisionVsOBB(OBB other)
    {
        // same as aabb
        // multiply circle center by box world matrix inverse
        return false;
    }
}
