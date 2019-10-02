using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OBB : CollisionHull2D
{
    public Vector2 halfWidths;
    protected Vector2[] axis;
    private void Start()
    {
        halfWidths.x = transform.localScale.x / 2;
        halfWidths.y = transform.localScale.y / 2;
        axis = new Vector2[2];
        axis[0] = new Vector2(Mathf.Cos(particle.rotation), Mathf.Sin(particle.rotation));
        axis[1] = new Vector2(-Mathf.Sin(particle.rotation), Mathf.Sin(particle.rotation));
    }

    private void Update()
    {
        axis[0] = new Vector2(Mathf.Cos(particle.rotation), Mathf.Sin(particle.rotation));
        axis[1] = new Vector2(-Mathf.Sin(particle.rotation), Mathf.Sin(particle.rotation));
    }

    public OBB( Vector2 halfWidths):base(CollisionHullType2D.OBB)
    {
        this.halfWidths = halfWidths;
        axis[0] = new Vector2(Mathf.Cos(particle.rotation), Mathf.Sin(particle.rotation));
        axis[1] = new Vector2(-Mathf.Sin(particle.rotation), Mathf.Sin(particle.rotation));
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

        Vector2[] corners = new Vector2[4];
        Matrix4x4 localToWorldMat = transform.localToWorldMatrix, localToWorldMatOther = other.particle.transform.localToWorldMatrix;

        // rotated point extents
        corners[0] = localToWorldMat.MultiplyPoint3x4(-halfWidths);
        corners[1] = localToWorldMat.MultiplyPoint3x4(new Vector2(halfWidths.x, -halfWidths.y));
        corners[2] = localToWorldMat.MultiplyPoint3x4(new Vector2(-halfWidths.x, halfWidths.y));
        corners[3] = localToWorldMat.MultiplyPoint3x4(halfWidths);

        // aabb around the obb points
        Vector2[] obbAsAABB = new Vector2[2];
        GetExtentsVector(ref obbAsAABB, corners);

        // aabb extents for obb
        Vector2 obbMinExt = particle.position - halfWidths, obbMaxExt = particle.position + halfWidths;

        // aabb extents
        Vector2 aabbMinExt = other.particle.position - other.halfWidths, aabbMaxExt = other.particle.position + other.halfWidths;

        // rotated corners
        corners[0] = (other.particle.position - particle.position) + (Vector2)localToWorldMat.MultiplyPoint3x4(-other.halfWidths);
        corners[1] = (other.particle.position - particle.position) + (Vector2)localToWorldMat.MultiplyPoint3x4(new Vector2(other.halfWidths.x, -other.halfWidths.y));
        corners[2] = (other.particle.position - particle.position) + (Vector2)localToWorldMat.MultiplyPoint3x4(new Vector2(-other.halfWidths.x, other.halfWidths.y));
        corners[3] = (other.particle.position - particle.position) + (Vector2)localToWorldMat.MultiplyPoint3x4(other.halfWidths);

        // obb aabb extents
        Vector2[] aabbAsOBB = new Vector2[2];
        GetExtentsVector(ref aabbAsOBB, corners);

        corner11 = aabbAsOBB[0];
        corner12 = aabbAsOBB[1];
        corner13 = obbAsAABB[0];
        corner14 = obbAsAABB[1];

        corner21 = aabbMinExt;
        corner22 = aabbMaxExt;
        corner23 = obbMinExt;
        corner24 = obbMaxExt;

        // check the (obb)aabb vs aabb
        bool firstCheck = (obbAsAABB[1].x >= aabbMinExt.x && obbAsAABB[1].y >= aabbMinExt.y) &&
                            (aabbMaxExt.x >= obbAsAABB[0].x && aabbMaxExt.y >= obbAsAABB[0].y);

        // check the obb(aabb) vs obb(aabb)
        bool secondCheck = (aabbAsOBB[1].x >= obbMinExt.x && aabbAsOBB[1].y >= obbMinExt.y) &&
                            (obbMaxExt.x >= aabbAsOBB[0].x && obbMaxExt.y >= aabbAsOBB[0].y);

        return firstCheck && secondCheck;
    }

    Vector2 corner11, corner12, corner13, corner14;
    Vector2 corner21, corner22, corner23, corner24;


    protected override bool TestCollisionVsOBB(OBB other)
    {
        // 1. get corner points of obb and other obb
        // 2. project other corner points onto axis[0]
        // 3. store min and max points (on line) for this and other
        // 4. do aabb between these points (if aabb says no collision then they're not colliding, this is for any axis)
        // 5. repeat 1-4 for axis[1]
        // 6. repeat 1-5 for other

        Vector2[] obbCorners = new Vector2[4];
        Vector2[] obbCornersOther = new Vector2[4];
        Vector2[] projectedPoints = new Vector2[4];
        // 0: min, 1: max
        Vector2[] extents = new Vector2[2], extentsOther = new Vector2[2];

        Matrix4x4 localToWorldMat = transform.localToWorldMatrix, localToWorldMatOther = other.particle.transform.localToWorldMatrix;

        corner11 = obbCorners[0] = localToWorldMat.MultiplyPoint3x4(-halfWidths);
        corner12 = obbCorners[1] = localToWorldMat.MultiplyPoint3x4(new Vector2(halfWidths.x, -halfWidths.y));
        corner13 = obbCorners[2] = localToWorldMat.MultiplyPoint3x4(new Vector2(-halfWidths.x, halfWidths.y));
        corner14 = obbCorners[3] = localToWorldMat.MultiplyPoint3x4(halfWidths);

        corner21 = obbCornersOther[0] = localToWorldMatOther.MultiplyPoint3x4(-other.halfWidths);
        corner22 = obbCornersOther[1] = localToWorldMatOther.MultiplyPoint3x4(new Vector2(other.halfWidths.x, -other.halfWidths.y));
        corner23 = obbCornersOther[2] = localToWorldMatOther.MultiplyPoint3x4(new Vector2(-other.halfWidths.x, other.halfWidths.y));
        corner24 = obbCornersOther[3] = localToWorldMatOther.MultiplyPoint3x4(other.halfWidths);

        bool aabbTest;

        for (int i = 0; i < axis.Length; ++i)
        {
            ProjectFourPoints(ref projectedPoints, obbCorners, axis[i]);
            GetExtentsVector(ref extents, projectedPoints);
            ProjectFourPoints(ref projectedPoints, obbCornersOther, axis[i]);
            GetExtentsVector(ref extentsOther, projectedPoints);

            aabbTest = extents[1].x >= extentsOther[0].x && extents[1].y >= extentsOther[0].y &&
                        extentsOther[1].x >= extents[0].x && extentsOther[1].y >= extents[0].y;

            if(aabbTest)
            {
                return false;
            }
        }

        for(int i = 0; i < other.axis.Length; ++i)
        {
            ProjectFourPoints(ref projectedPoints, obbCorners, other.axis[i]);
            GetExtentsVector(ref extents, projectedPoints);
            ProjectFourPoints(ref projectedPoints, obbCornersOther, other.axis[i]);
            GetExtentsVector(ref extentsOther, projectedPoints);

            aabbTest = extents[1].x >= extentsOther[0].x && extents[1].y >= extentsOther[0].y &&
                        extentsOther[1].x >= extents[0].x && extentsOther[1].y >= extents[0].y;

            if (aabbTest)
            {
                return false;
            }
        }

        return true;
    }

    private void GetExtentsVector(ref Vector2[] extents, Vector2[] points)
    {
        Vector2 max = new Vector2(Mathf.NegativeInfinity, Mathf.NegativeInfinity), min = new Vector2(Mathf.Infinity, Mathf.Infinity);
        for(int i = 0; i < points.Length; ++i)
        {
            max.x = Mathf.Max(max.x, points[i].x);
            max.y = Mathf.Max(max.y, points[i].y);
            min.x = Mathf.Min(min.x, points[i].x);
            min.y = Mathf.Min(min.y, points[i].y);
        }
        extents[0] = min;
        extents[1] = max;
    }

    private void ProjectFourPoints(ref Vector2[] projectedPoints, Vector2[] pointsToProject, Vector2 axis)
    {
        for(int i = 0; i < projectedPoints.Length; ++i)
        {
            projectedPoints[i] = ProjectPoint(pointsToProject[i], axis);
        }
    }

    private Vector2 ProjectPoint(Vector2 point, Vector2 normal)
    {
        return Vector2.Dot(point, normal) * normal;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(corner11, .1f);
        Gizmos.DrawSphere(corner12, .1f);
        Gizmos.DrawSphere(corner13, .1f);
        Gizmos.DrawSphere(corner14, .1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(corner21, .1f);
        Gizmos.DrawSphere(corner22, .1f);
        Gizmos.DrawSphere(corner23, .1f);
        Gizmos.DrawSphere(corner24, .1f);
    }
}
