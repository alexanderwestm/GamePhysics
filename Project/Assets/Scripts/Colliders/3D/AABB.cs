using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AABB : CollisionHull3D
{
    public Vector3 halfWidths;

    public AABB(Vector3 halfWidths) : base(CollisionHullType3D.AABB)
    {
        this.halfWidths = halfWidths;
    }

    private void Start()
    {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (meshFilter != null)
        {
            halfWidths.x = meshFilter.mesh.bounds.size.x * transform.localScale.x * .5f;
            halfWidths.y = meshFilter.mesh.bounds.size.y * transform.localScale.y * .5f;
            halfWidths.z = meshFilter.mesh.bounds.size.z * transform.localScale.z * .5f;
        }
        else if(renderer != null)
        {
            halfWidths.x = renderer.sprite.bounds.size.x * transform.localScale.x * .5f;
            halfWidths.y = renderer.sprite.bounds.size.y * transform.localScale.y * .5f;
            halfWidths.z = renderer.sprite.bounds.size.z * transform.localScale.z * .5f;
        }

        type = CollisionHullType3D.AABB;
    }

    protected override bool TestCollisionVsCircle(Sphere other, out Collision collision)
    {
        return CollisionHull3D.TestCollision(other, this, out collision);
    }

    protected override bool TestCollisionVsAABB(AABB other, out Collision collision)
    {
        collision = null;
        // if distance between centers is greater than half widths they're not colliding
        // component wise check
        // 1. store abs(distance) on x, y
        // 2. store width on x, y
        // 3. return if distance is less than width

        Vector3 ourCenter = particle.position, otherCenter = other.particle.position;
        Vector3 dist = ourCenter - otherCenter;
        dist.x = Mathf.Abs(dist.x);
        dist.y = Mathf.Abs(dist.y);
        dist.z = Mathf.Abs(dist.z);

        Vector3 sumHalfWidths = halfWidths + other.halfWidths;
        return dist.x < sumHalfWidths.x && dist.y < sumHalfWidths.y && dist.z < sumHalfWidths.z;
    }

    Vector3 point1, point2, point3, point4, point5, point6, point7, point8, point9, point10;

    // TO DO
    protected override bool TestCollisionVsOBB(OBB other, out Collision collision)
    {
        collision = null;
        // Code from Michael Zheng and Ben Strong, using with permission


        // same as above twice
        // find max extents of OBB, do ABB vs this box
        // then, transform this box into OBB's space, find max extents, repeat

        //1. create max and min extents of this particle
        //2. max = (center.x + length/2, center.y + height/2)
        //3. min = (center.x - length/2, center.y - height/2)
        //4. create max and min extents of other particle
        //4a. find all corner points of the box using length, height
        //4b. rotate all points around its rotation https://www.gamefromscratch.com/post/2012/11/24/GameDev-math-recipes-Rotating-one-point-around-another-point.aspx
        //4c. min = (min(x of all points), min(y of all points)) https://stackoverflow.com/questions/3231176/how-to-get-size-of-a-rotated-rectangle
        //4d. max = (max(x of all points), max(y of all points)
        //5. check if this_max.x > other_min.x and this_max.y > other_min.y
        //6. check if other_max.x > this_min.x and other_max.y > this_min.y
        //7. transform this center around the other's center using its transform matrix inverse
        //8. transform each point of the aab by the other's transform matrix inverse
        //8. find max and min extents of this by using Max(all points), Min(all points)
        //5. check if new this_max.x > other_min.x and new this_max.y > other_min.y
        //6. check if other_max.x > new this_min.x and other_max.y > new this_min.y
        //9. if all checks are true, then collision check passes

        Matrix4x4 otherMatrix = other.transform.worldToLocalMatrix;//other.particle.worldTransformMatrixInverse;

        bool check1, check2;
        Vector3 thisMax, thisMin, otherMax, otherMin;
        Vector3 p1, p2, p3, p4, p5, p6, p7, p8;

        Vector3 otherPosition = other.particle.position;
        Vector3 thisWidths = halfWidths;
        Vector3 otherWidths = other.halfWidths;

        //max and min of this position
        point9 = thisMax = particle.position + thisWidths;
        point10 = thisMin = particle.position - thisWidths;

        //find max and min of other
        //get all corner points and then rotate it
        //p1 = rotatePoint(new Vector2(otherLength, otherHeight), other.rotation);
        //p2 = rotatePoint(new Vector2(otherLength, -otherHeight), other.rotation);
        //p3 = rotatePoint(new Vector2(-otherLength, -otherHeight), other.rotation);
        //p4 = rotatePoint(new Vector2(-otherLength, otherHeight), other.rotation);
        //p1 = other.transform.worldToLocalMatrix * (new Vector2(otherLength, otherHeight));
        //p2 = other.transform.worldToLocalMatrix * (new Vector2(otherLength, -otherHeight));
        //p3 = other.transform.worldToLocalMatrix * (new Vector2(-otherLength, -otherHeight));
        //p4 = other.transform.worldToLocalMatrix * (new Vector2(-otherLength, otherHeight));

        point1 = p1 = otherMatrix.MultiplyPoint3x4(new Vector3(otherWidths.x, otherWidths.y, otherWidths.z));
        point2 = p2 = otherMatrix.MultiplyPoint3x4(new Vector3(otherWidths.x, otherWidths.y, -otherWidths.z));
        point3 = p3 = otherMatrix.MultiplyPoint3x4(new Vector3(otherWidths.x, -otherWidths.y, otherWidths.z));
        point4 = p4 = otherMatrix.MultiplyPoint3x4(new Vector3(otherWidths.x, -otherWidths.y, -otherWidths.z));
        point5 = p5 = otherMatrix.MultiplyPoint3x4(new Vector3(-otherWidths.x, otherWidths.y, otherWidths.z));
        point6 = p6 = otherMatrix.MultiplyPoint3x4(new Vector3(-otherWidths.x, otherWidths.y, -otherWidths.z));
        point7 = p7 = otherMatrix.MultiplyPoint3x4(new Vector3(-otherWidths.x, -otherWidths.y, otherWidths.z));
        point8 = p8 = otherMatrix.MultiplyPoint3x4(new Vector3(-otherWidths.x, -otherWidths.y, -otherWidths.z));

        //find max of all points
        otherMax = new Vector3(Mathf.Max(p1.x, p2.x, p3.x, p4.x, p5.x, p6.x, p7.x, p8.x) + otherPosition.x, Mathf.Max(p1.y, p2.y, p3.y, p4.y, p5.y, p6.y, p7.y, p8.y) + otherPosition.y, Mathf.Max(p1.z, p2.z, p3.z, p4.z, p5.z, p6.z, p7.z, p8.z) + otherPosition.z);
        otherMin = new Vector3(Mathf.Min(p1.x, p2.x, p3.x, p4.x, p5.x, p6.x, p7.x, p8.x) + otherPosition.x, Mathf.Min(p1.y, p2.y, p3.y, p4.y, p5.y, p6.y, p7.y, p8.y) + otherPosition.y, Mathf.Min(p1.z, p2.z, p3.z, p4.z, p5.z, p6.z, p7.z, p8.z) + otherPosition.z);

        // max > min && max > min
        check1 = (thisMax.x >= otherMin.x && thisMax.y >= otherMin.y && thisMax.z >= otherMax.z) && (otherMax.x >= thisMin.x && otherMax.y >= thisMin.y && otherMax.z >= thisMax.z);

        if(!check1)
        {
            return false;
        }

        //for each corner, move it relative to the box then transform by the world matrix inverse. Finally add position back
        //p1 = other.transform.worldToLocalMatrix * (new Vector2(particle.position.x + thisLength, particle.position.y + thisHeight) - otherPosition); //this one
        //p2 = other.transform.worldToLocalMatrix * (new Vector2(particle.position.x + thisLength, particle.position.y - thisHeight) - otherPosition);
        //p3 = other.transform.worldToLocalMatrix * (new Vector2(particle.position.x - thisLength, particle.position.y - thisHeight) - otherPosition);
        //p4 = other.transform.worldToLocalMatrix * (new Vector2(particle.position.x - thisLength, particle.position.y + thisHeight) - otherPosition);
        //p1 += otherPosition;
        //p2 += otherPosition;
        //p3 += otherPosition;
        //p4 += otherPosition;
        p1 = otherMatrix * (new Vector3(particle.position.x + halfWidths.x, particle.position.y + halfWidths.y, particle.position.z + halfWidths.z) - otherPosition);
        p2 = otherMatrix * (new Vector3(particle.position.x + halfWidths.x, particle.position.y + halfWidths.y, particle.position.z - halfWidths.z) - otherPosition);
        p3 = otherMatrix * (new Vector3(particle.position.x + halfWidths.x, particle.position.y - halfWidths.y, particle.position.z + halfWidths.z) - otherPosition);
        p4 = otherMatrix * (new Vector3(particle.position.x + halfWidths.x, particle.position.y - halfWidths.y, particle.position.z - halfWidths.z) - otherPosition);
        p5 = otherMatrix * (new Vector3(particle.position.x - halfWidths.x, particle.position.y + halfWidths.y, particle.position.z + halfWidths.z) - otherPosition);
        p6 = otherMatrix * (new Vector3(particle.position.x - halfWidths.x, particle.position.y + halfWidths.y, particle.position.z - halfWidths.z) - otherPosition);
        p7 = otherMatrix * (new Vector3(particle.position.x - halfWidths.x, particle.position.y - halfWidths.y, particle.position.z + halfWidths.z) - otherPosition);
        p8 = otherMatrix * (new Vector3(particle.position.x - halfWidths.x, particle.position.y - halfWidths.y, particle.position.z - halfWidths.z) - otherPosition);
        p1 += otherPosition;
        p2 += otherPosition;
        p3 += otherPosition;
        p4 += otherPosition;
        p5 += otherPosition;
        p6 += otherPosition;
        p7 += otherPosition;
        p8 += otherPosition;

        //get the extremes for min and max
        //thisMax = new Vector2(Mathf.Max(p1.x, p2.x, p3.x, p4.x), Mathf.Max(p1.y, p2.y, p3.y, p4.y));
        //thisMin = new Vector2(Mathf.Min(p1.x, p2.x, p3.x, p4.x), Mathf.Min(p1.y, p2.y, p3.y, p4.y));
        thisMax = new Vector3(Mathf.Max(p1.x, p2.x, p3.x, p4.x, p5.x, p6.x, p7.x, p8.x) + otherPosition.x, Mathf.Max(p1.y, p2.y, p3.y, p4.y, p5.y, p6.y, p7.y, p8.y) + otherPosition.y, Mathf.Max(p1.z, p2.z, p3.z, p4.z, p5.z, p6.z, p7.z, p8.z) + otherPosition.z);
        thisMin = new Vector3(Mathf.Min(p1.x, p2.x, p3.x, p4.x, p5.x, p6.x, p7.x, p8.x) + otherPosition.x, Mathf.Min(p1.y, p2.y, p3.y, p4.y, p5.y, p6.y, p7.y, p8.y) + otherPosition.y, Mathf.Min(p1.z, p2.z, p3.z, p4.z, p5.z, p6.z, p7.z, p8.z) + otherPosition.z);

        //otherMax = new Vector2(otherPosition.x + otherLength, otherPosition.y + otherHeight);
        //otherMin = new Vector2(otherPosition.x - otherLength, otherPosition.y - otherHeight);

        otherMax = otherPosition + otherWidths;
        otherMin = otherPosition - otherWidths;

        check2 = (thisMax.x >= otherMin.x && thisMax.y >= otherMin.y && thisMax.z >= otherMax.z) && (otherMax.x >= thisMin.x && otherMax.y >= thisMin.y && otherMax.z > thisMax.z);

        if(!check2)
        {
            return false;
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(point1, .1f);
        Gizmos.DrawWireSphere(point2, .1f);
        Gizmos.DrawWireSphere(point3, .1f);
        Gizmos.DrawWireSphere(point4, .1f);
        Gizmos.DrawWireSphere(point5, .1f);
        Gizmos.DrawWireSphere(point6, .1f);
        Gizmos.DrawWireSphere(point7, .1f);
        Gizmos.DrawWireSphere(point8, .1f);
        Gizmos.DrawWireSphere(point9, .1f);
        Gizmos.DrawWireSphere(point10, .1f);
    }
}
