using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OBB : CollisionHull2D
{
    public Vector2 halfWidths;

    public OBB( Vector2 halfWidths):base(CollisionHullType2D.OBB)
    {
        this.halfWidths = halfWidths;
    }

    protected override bool TestCollisionVsCircle(Circle other)
    {
        // same as aabb
        // multiply circle center by box world matrix inverse
        // other.transform.worldToLocalMatrix

        // 1. other center = transform.worldtolocalmatrix * center
        // 2. do aabb check
        // max extents are axis * halfwidths + position

        Vector2 minExtents = particle.position - halfWidths, maxExtents = particle.position + halfWidths;
        Vector2 adjustedCenter = transform.InverseTransformPoint(other.particle.position);
        adjustedCenter += particle.position;

        Vector2 closestPoint;

        closestPoint.x = Mathf.Clamp(adjustedCenter.x, minExtents.x, maxExtents.x);
        closestPoint.y = Mathf.Clamp(adjustedCenter.y, minExtents.y, maxExtents.y);

        Vector2 delta = adjustedCenter - closestPoint;

        return delta.sqrMagnitude < other.radius * other.radius;
    }

    Vector2 obbMin, obbMax, aabbMin, aabbMax;
    Vector2 obbAABBMin, obbAABBMax, aabbOBBMin, aabbOBBMax;

    protected override bool TestCollisionVsAABB(AABB other)
    {
        // same as aabb aabb twice
        // find max extents of OBB, do AABB vs this box
        // then, transform this box into OBB's space, find max extents, repeat
        // 1. get min max extents of rotated  obb
        // 2. get min max extents of aabb
        // 3. get min max extents of rotated aabb
        // 4. get min max extents of obb
        // 5. do aabb check between 1/2
        // 6. do aabb check between 3/4
        // 7. if either fails they collide

        Vector2 bottomLeft, bottomRight, topLeft, topRight;

        bottomLeft = transform.InverseTransformPoint(particle.position - halfWidths);
        bottomRight = transform.InverseTransformPoint(particle.position + new Vector2(halfWidths.x, -halfWidths.y));
        topLeft = transform.InverseTransformPoint(particle.position + new Vector2(-halfWidths.x, halfWidths.y));
        topRight = transform.InverseTransformPoint(particle.position + halfWidths);

        Vector2 obbAsAABBMinExt = new Vector2(Mathf.Min(bottomLeft.x, Mathf.Min(bottomRight.x, Mathf.Min(topLeft.x, topRight.x))),
                                Mathf.Min(bottomLeft.y, Mathf.Min(bottomRight.y, Mathf.Min(topLeft.y, topRight.y)))), 
                obbAsAABBMaxExt = new Vector2(Mathf.Max(bottomLeft.x, Mathf.Max(bottomRight.x, Mathf.Max(topLeft.x, topRight.x))),
                                Mathf.Max(bottomLeft.y, Mathf.Max(bottomRight.y, Mathf.Max(topLeft.y, topRight.y))));

        Vector2 obbMinExt = -halfWidths;
        Vector2 obbMaxExt = halfWidths;

        Vector2 aabbMinExt = -other.halfWidths, aabbMaxExt = other.halfWidths;

        Vector2 aabbAsOBBMinExt = particle.position + (Vector2)transform.InverseTransformPoint(aabbMinExt);
        Vector2 aabbAsOBBMaxExt = particle.position + (Vector2)transform.InverseTransformPoint(aabbMaxExt);

        bottomLeft = transform.InverseTransformPoint(other.particle.position - other.halfWidths);
        bottomRight = transform.InverseTransformPoint(other.particle.position + new Vector2(other.halfWidths.x, -other.halfWidths.y));
        topLeft = transform.InverseTransformPoint(other.particle.position + new Vector2(-other.halfWidths.x, other.halfWidths.y));
        topRight = transform.InverseTransformPoint(other.particle.position + other.halfWidths);

        aabbAsOBBMinExt = new Vector2(Mathf.Min(bottomLeft.x, Mathf.Min(bottomRight.x, Mathf.Min(topLeft.x, topRight.x))),
                                Mathf.Min(bottomLeft.y, Mathf.Min(bottomRight.y, Mathf.Min(topLeft.y, topRight.y))));
        aabbAsOBBMaxExt = new Vector2(Mathf.Max(bottomLeft.x, Mathf.Max(bottomRight.x, Mathf.Max(topLeft.x, topRight.x))),
                                Mathf.Max(bottomLeft.y, Mathf.Max(bottomRight.y, Mathf.Max(topLeft.y, topRight.y))));

        // this one is right
        bool firstCheck = (obbAsAABBMaxExt.x >= aabbMinExt.x && obbAsAABBMaxExt.y >= aabbMinExt.y) &&
                            (aabbMaxExt.x >= obbAsAABBMinExt.x && aabbMaxExt.y >= obbAsAABBMinExt.y);

        bool secondCheck = (aabbAsOBBMaxExt.x >= obbMinExt.x && aabbAsOBBMaxExt.y >= obbMinExt.y) &&
                            (obbMaxExt.x >= aabbAsOBBMinExt.x && obbMaxExt.y >= aabbAsOBBMinExt.y);

        return firstCheck && secondCheck;
    }
    protected override bool TestCollisionVsOBB(OBB other)
    {
        // 1. get corner points of obb and other obb
        // 2. project other corner points onto axis[0]
        // 3. store min and max points (on line) for this and other
        // 4. do aabb between these points (if aabb says no collision then they're not colliding, this is for any axis)
        // 5. repeat 1-4 for axis[1]
        // 6. repeat 1-5 for other
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(obbMin, .1f);
        Gizmos.DrawWireSphere(obbMax, .1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(aabbMin, .1f);
        Gizmos.DrawWireSphere(aabbMax, .1f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(obbAABBMin, .1f);
        Gizmos.DrawWireSphere(obbAABBMax, .1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(aabbOBBMin, .1f);
        Gizmos.DrawWireSphere(aabbOBBMax, .1f);
    }
}
