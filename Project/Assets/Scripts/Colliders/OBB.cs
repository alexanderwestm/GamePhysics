using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OBB : CollisionHull2D
{
    [SerializeField] Vector2[] axis = new Vector2[2];

    public Vector2 halfWidths;

    public OBB(Vector2[] axis, Vector2 halfWidths):base(CollisionHullType2D.OBB)
    {
        this.axis = axis;
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

        Vector2 obbAsAABBMinExt = particle.position - halfWidths, obbAsAABBMaxExt = particle.position + halfWidths;
        Vector2 aabbMinExt = other.particle.position - other.halfWidths, aabbMaxExt = other.particle.position + other.halfWidths;
        Vector2 aabbAsOBBPos = transform.InverseTransformPoint(other.particle.position);
        Vector2 aabbAsOBBMinExt = transform.InverseTransformPoint(aabbMinExt);
        Vector2 aabbAsOBBMaxExt = transform.InverseTransformPoint(aabbMaxExt);

        Vector2 obbMinExt = transform.InverseTransformPoint(obbAsAABBMinExt);
        Vector2 obbMaxExt = transform.InverseTransformPoint(obbAsAABBMaxExt);

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
