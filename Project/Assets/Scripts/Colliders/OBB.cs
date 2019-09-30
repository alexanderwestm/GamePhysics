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

        Vector2 bottomLeft, bottomRight, topLeft, topRight;

        bottomLeft = transform.InverseTransformPoint(particle.position - halfWidths);
        bottomRight = transform.InverseTransformPoint(particle.position + new Vector2(halfWidths.x, -halfWidths.y));
        topLeft = transform.InverseTransformPoint(particle.position + new Vector2(-halfWidths.x, halfWidths.y));
        topRight = transform.InverseTransformPoint(particle.position + halfWidths);

        Vector2 obbAsAABBMinExt = new Vector2(Mathf.Min(bottomLeft.x, Mathf.Min(bottomRight.x, Mathf.Min(topLeft.x, topRight.x))),
                                Mathf.Min(bottomLeft.y, Mathf.Min(bottomRight.y, Mathf.Min(topLeft.y, topRight.y)))) + particle.position, 
                obbAsAABBMaxExt = new Vector2(Mathf.Max(bottomLeft.x, Mathf.Max(bottomRight.x, Mathf.Max(topLeft.x, topRight.x))),
                                Mathf.Max(bottomLeft.y, Mathf.Max(bottomRight.y, Mathf.Max(topLeft.y, topRight.y)))) + particle.position;

        Vector2 obbMinExt = particle.position + (Vector2)transform.InverseTransformPoint(particle.position - halfWidths);
        Vector2 obbMaxExt = particle.position + (Vector2)transform.InverseTransformPoint(particle.position + halfWidths);

        Vector2 aabbMinExt = other.particle.position - other.halfWidths, aabbMaxExt = other.particle.position + other.halfWidths;

        Vector2 aabbAsOBBMinExt = particle.position + (Vector2)transform.InverseTransformPoint(aabbMinExt);
        Vector2 aabbAsOBBMaxExt = particle.position + (Vector2)transform.InverseTransformPoint(aabbMaxExt);



        bool firstCheck = (obbAsAABBMaxExt.x >= aabbMinExt.x && obbAsAABBMaxExt.y >= aabbMinExt.y) &&
                            (aabbMaxExt.x >= obbAsAABBMinExt.x && aabbMaxExt.y >= obbAsAABBMinExt.y);

        bool secondCheck = (aabbAsOBBMaxExt.x >= obbMinExt.x && aabbAsOBBMaxExt.y >= obbMinExt.y) &&
                            (obbMaxExt.x >= aabbAsOBBMinExt.x && obbMaxExt.y >= aabbAsOBBMinExt.y);

        obbMin = obbAsAABBMinExt;
        obbMax = obbAsAABBMaxExt;
        aabbMin = aabbMinExt;
        aabbMax = aabbMaxExt;
        obbAABBMin = obbMinExt;
        obbAABBMax = obbMaxExt;
        aabbOBBMin = aabbAsOBBMinExt;
        aabbOBBMax = aabbAsOBBMaxExt;

        return firstCheck && secondCheck;
    }
    protected override bool TestCollisionVsOBB(OBB other)
    {
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
