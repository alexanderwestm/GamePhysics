using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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

        Vector2 point, otherMin, otherMax, delta;
        otherMin = other.particle.position - other.halfWidths;
        otherMax = other.particle.position + other.halfWidths;

        // find the closest point on the aabb
        point.x = Mathf.Clamp(particle.position.x, otherMin.x, otherMax.x);
        point.y = Mathf.Clamp(particle.position.y, otherMin.y, otherMax.y);

        delta = particle.position - point;

        // point collision with circle test
        return delta.sqrMagnitude < radius * radius;
    }

    protected override bool TestCollisionVsOBB(OBB other)
    {
        // same as aabb
        // multiply circle center by box world matrix inverse
        //other.transform.worldToLocalMatrix
        Vector2 minExtents = other.particle.position - other.halfWidths, maxExtents = other.particle.position + other.halfWidths;
        Vector2 adjustedCenter = other.transform.InverseTransformPoint(particle.position);
        adjustedCenter += other.particle.position;

        Vector2 closestPoint;

        closestPoint.x = Mathf.Clamp(adjustedCenter.x, minExtents.x, maxExtents.x);
        closestPoint.y = Mathf.Clamp(adjustedCenter.y, minExtents.y, maxExtents.y);

        Vector2 delta = adjustedCenter - closestPoint;

        return delta.sqrMagnitude < radius * radius;
    }
}
