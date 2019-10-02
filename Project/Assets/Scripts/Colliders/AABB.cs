using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AABB : CollisionHull2D
{
    public Vector2 halfWidths;

    public AABB(Vector2 halfWidths) : base(CollisionHullType2D.AABB)
    {
        this.halfWidths = halfWidths;
    }

    protected override bool TestCollisionVsCircle(Circle other)
    {
        // see circle

        Vector2 point, otherMin, otherMax, delta;
        otherMin = particle.position - halfWidths;
        otherMax = particle.position + halfWidths;

        // find the closest point on the aabb
        point.x = Mathf.Clamp(other.particle.position.x, otherMin.x, otherMax.x);
        point.y = Mathf.Clamp(other.particle.position.y, otherMin.y, otherMax.y);

        delta = particle.position - point;

        // point collision with circle test
        return delta.sqrMagnitude < other.radius * other.radius;
    }

    protected override bool TestCollisionVsAABB(AABB other)
    {
        // if distance between centers is greater than half widths they're not colliding
        // component wise check
        // 1. store abs(distance) on x, y
        // 2. store width on x, y
        // 3. return if distance is less than width

        Vector2 ourCenter = particle.position, otherCenter = other.particle.position;
        Vector2 dist = ourCenter - otherCenter;
        dist.x = Mathf.Abs(dist.x);
        dist.y = Mathf.Abs(dist.y);

        Vector2 sumHalfWidths = halfWidths + other.halfWidths;
        return dist.x < sumHalfWidths.x && dist.y < sumHalfWidths.y;
    }

    protected override bool TestCollisionVsOBB(OBB other)
    {
        // same as above twice
        // find max extents of OBB, do ABB vs this box
        // then, transform this box into OBB's space, find max extents, repeat
        return false;
    }
}
