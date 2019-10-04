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

        //Vector2 minExtents = particle.position - halfWidths, maxExtents = particle.position + halfWidths;
        //Vector2 adjustedCenter = transform.InverseTransformPoint(other.particle.position);
        //adjustedCenter += particle.position;
        //
        //Vector2 closestPoint;
        //
        //closestPoint.x = Mathf.Clamp(adjustedCenter.x, minExtents.x, maxExtents.x);
        //closestPoint.y = Mathf.Clamp(adjustedCenter.y, minExtents.y, maxExtents.y);
        //
        //Vector2 delta = adjustedCenter - closestPoint;
        //
        //return delta.sqrMagnitude < other.radius * other.radius;

        return CollisionHull2D.TestCollision(other, this);
    }

    protected override bool TestCollisionVsAABB(AABB other)
    {
        return CollisionHull2D.TestCollision(other, this);
    }

    protected override bool TestCollisionVsOBB(OBB other)
    {
        // Code from Michael Zheng and Ben Strong, using with permission

        //Transform this into others space and do AABB vs OBB
        //transform other into this space and do AABB vs OBB
        //If both tests pass, collision occurs otherwise no collision

        //1. do AABB check with this rotated around other
        //2. find all corner points of this box using length and height
        //3. rotate the points of this box with its rotation
        //4. transform the points using the transform inverse of the other
        //5. min = (min(x of all points), min(y of all points))
        //6. max = (max(x of all points), max(y of all points)
        //7. create max and min extents of other particle
        //8. max = (center.x + length/2, center.y + height/2)
        //9. min = (center.x - length/2, center.y - height/2)
        //10. check if this_max.x > other_min.x and this_max.y > other_min.y
        //11. check if other_max.x > this_min.x and other_max.y > this_min.y 
        //12. do AABB check with other rotated around this
        //13. find all corner points of other box using length and height
        //14. rotate the points of other box with its rotation
        //15. transform the points using the transform inverse of this
        //16. min = (min(x of all points), min(y of all points))
        //17. max = (max(x of all points), max(y of all points)
        //18. create max and min extents of this particle
        //19. max = (center.x + length/2, center.y + height/2)
        //20. min = (center.x - length/2, center.y - height/2)
        //21. check if this_max.x > other_min.x and this_max.y > other_min.y
        //22. check if other_max.x > this_min.x and other_max.y > this_min.y 

        bool check1, check2;
        Vector2 thisMax, thisMin, otherMax, otherMin;
        Vector2 p1, p2, p3, p4;

        Vector2 otherPosition = other.particle.position;
        float thisLength = halfWidths[0];
        float thisHeight = halfWidths[1];
        float otherLength = other.halfWidths[0];
        float otherHeight = other.halfWidths[1];

        //get all corner points and then rotate it

        p1 = transform.worldToLocalMatrix * (new Vector2(thisLength, thisHeight));
        p2 = transform.worldToLocalMatrix * (new Vector2(thisLength, -thisHeight));
        p3 = transform.worldToLocalMatrix * (new Vector2(-thisLength, -thisHeight));
        p4 = transform.worldToLocalMatrix * (new Vector2(-thisLength, thisHeight));
        //for each corner, move it relative to the box then transform by the world matrix inverse. Finally add position back
        p1 = other.transform.worldToLocalMatrix * (p1 + particle.position - otherPosition);
        p2 = other.transform.worldToLocalMatrix * (p2 + particle.position - otherPosition);
        p3 = other.transform.worldToLocalMatrix * (p3 + particle.position - otherPosition);
        p4 = other.transform.worldToLocalMatrix * (p4 + particle.position - otherPosition);

        p1 += otherPosition;
        p2 += otherPosition;
        p3 += otherPosition;
        p4 += otherPosition;

        thisMax = new Vector2(Mathf.Max(p1.x, p2.x, p3.x, p4.x), Mathf.Max(p1.y, p2.y, p3.y, p4.y));
        thisMin = new Vector2(Mathf.Min(p1.x, p2.x, p3.x, p4.x), Mathf.Min(p1.y, p2.y, p3.y, p4.y));

        otherMax = new Vector2(otherPosition.x + otherLength, otherPosition.y + otherHeight);
        otherMin = new Vector2(otherPosition.x - otherLength, otherPosition.y - otherHeight);

        check1 = (thisMax.x >= otherMin.x && thisMax.y >= otherMin.y) && (otherMax.x >= thisMin.x && otherMax.y >= thisMin.y);

        if (!check1)
        {
            return false;
        }

        //get all corner points and then rotate it
        p1 = other.transform.worldToLocalMatrix * (new Vector2(otherLength, otherHeight));
        p2 = other.transform.worldToLocalMatrix * (new Vector2(otherLength, -otherHeight));
        p3 = other.transform.worldToLocalMatrix * (new Vector2(-otherLength, -otherHeight));
        p4 = other.transform.worldToLocalMatrix * (new Vector2(-otherLength, otherHeight));
        //for each corner, move it relative to the box then transform by the world matrix inverse. Finally add position back
        p1 = transform.worldToLocalMatrix * (p1 + otherPosition - particle.position); //this
        p2 = transform.worldToLocalMatrix * (p2 + otherPosition - particle.position);
        p3 = transform.worldToLocalMatrix * (p3 + otherPosition - particle.position);
        p4 = transform.worldToLocalMatrix * (p4 + otherPosition - particle.position);

        p1 += particle.position;
        p2 += particle.position;
        p3 += particle.position;
        p4 += particle.position;

        otherMax = new Vector2(Mathf.Max(p1.x, p2.x, p3.x, p4.x), Mathf.Max(p1.y, p2.y, p3.y, p4.y));
        otherMin = new Vector2(Mathf.Min(p1.x, p2.x, p3.x, p4.x), Mathf.Min(p1.y, p2.y, p3.y, p4.y));

        thisMax = new Vector2(particle.position.x + thisLength, particle.position.y + thisHeight);
        thisMin = new Vector2(particle.position.x - thisLength, particle.position.y - thisHeight);

        check2 = (thisMax.x >= otherMin.x && thisMax.y >= otherMin.y) && (otherMax.x >= thisMin.x && otherMax.y >= thisMin.y);

        if (!check2)
        {
            return false;
        }

        return true;
    }
}
