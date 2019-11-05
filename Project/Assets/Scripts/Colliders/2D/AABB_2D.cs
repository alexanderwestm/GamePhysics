using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AABB_2D : CollisionHull2D
{
    public Vector2 halfWidths { get; private set; }

    public AABB_2D(Vector2 halfWidths) : base(CollisionHullType2D.AABB)
    {
        this.halfWidths = halfWidths;
    }

    private void Start()
    {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Vector2 temp = new Vector2();
        if (meshFilter != null)
        {
            temp.x = meshFilter.mesh.bounds.size.x * transform.localScale.x / 2;
            temp.y = meshFilter.mesh.bounds.size.y * transform.localScale.y / 2;
        }
        else if(renderer != null)
        {
            temp.x = renderer.sprite.bounds.size.x * transform.localScale.x * .5f;
            temp.y = renderer.sprite.bounds.size.y * transform.localScale.y * .5f;
        }

        halfWidths = new Vector2(temp.x, temp.y);
        type = CollisionHullType2D.AABB;
    }

    protected override bool TestCollisionVsCircle(Circle other, out Collision collision)
    {
        return CollisionHull2D.TestCollision(other, this, out collision);
    }

    protected override bool TestCollisionVsAABB(AABB_2D other, out Collision collision)
    {
        collision = null;
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

    protected override bool TestCollisionVsOBB(OBB_2D other, out Collision collision)
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

        bool check1, check2;
        Vector2 thisMax, thisMin, otherMax, otherMin;
        Vector2 p1, p2, p3, p4;

        Vector2 otherPosition = other.particle.position;
        float thisLength = halfWidths[0];
        float thisHeight = halfWidths[1];
        float otherLength = other.halfWidths[0];
        float otherHeight = other.halfWidths[1];

        //max and min of this position
        thisMax = new Vector2(particle.position.x + thisLength, particle.position.y + thisHeight);
        thisMin = new Vector2(particle.position.x - thisLength, particle.position.y - thisHeight);

        //find max and min of other
        //get all corner points and then rotate it
        //p1 = rotatePoint(new Vector2(otherLength, otherHeight), other.rotation);
        //p2 = rotatePoint(new Vector2(otherLength, -otherHeight), other.rotation);
        //p3 = rotatePoint(new Vector2(-otherLength, -otherHeight), other.rotation);
        //p4 = rotatePoint(new Vector2(-otherLength, otherHeight), other.rotation);
        p1 = other.transform.worldToLocalMatrix * (new Vector2(otherLength, otherHeight));
        p2 = other.transform.worldToLocalMatrix * (new Vector2(otherLength, -otherHeight));
        p3 = other.transform.worldToLocalMatrix * (new Vector2(-otherLength, -otherHeight));
        p4 = other.transform.worldToLocalMatrix * (new Vector2(-otherLength, otherHeight));
        //find max of all points
        otherMax = new Vector2(Mathf.Max(p1.x, p2.x, p3.x, p4.x) + otherPosition.x, Mathf.Max(p1.y, p2.y, p3.y, p4.y) + otherPosition.y);
        otherMin = new Vector2(Mathf.Min(p1.x, p2.x, p3.x, p4.x) + otherPosition.x, Mathf.Min(p1.y, p2.y, p3.y, p4.y) + otherPosition.y);

        check1 = (thisMax.x >= otherMin.x && thisMax.y >= otherMin.y) && (otherMax.x >= thisMin.x && otherMax.y >= thisMin.y);

        if(!check1)
        {
            return false;
        }

        //for each corner, move it relative to the box then transform by the world matrix inverse. Finally add position back
        p1 = other.transform.worldToLocalMatrix * (new Vector2(particle.position.x + thisLength, particle.position.y + thisHeight) - otherPosition); //this one
        p2 = other.transform.worldToLocalMatrix * (new Vector2(particle.position.x + thisLength, particle.position.y - thisHeight) - otherPosition);
        p3 = other.transform.worldToLocalMatrix * (new Vector2(particle.position.x - thisLength, particle.position.y - thisHeight) - otherPosition);
        p4 = other.transform.worldToLocalMatrix * (new Vector2(particle.position.x - thisLength, particle.position.y + thisHeight) - otherPosition);
        p1 += otherPosition;
        p2 += otherPosition;
        p3 += otherPosition;
        p4 += otherPosition;

        //get the extremes for min and max
        thisMax = new Vector2(Mathf.Max(p1.x, p2.x, p3.x, p4.x), Mathf.Max(p1.y, p2.y, p3.y, p4.y));
        thisMin = new Vector2(Mathf.Min(p1.x, p2.x, p3.x, p4.x), Mathf.Min(p1.y, p2.y, p3.y, p4.y));

        otherMax = new Vector2(otherPosition.x + otherLength, otherPosition.y + otherHeight);
        otherMin = new Vector2(otherPosition.x - otherLength, otherPosition.y - otherHeight);

        check2 = (thisMax.x >= otherMin.x && thisMax.y >= otherMin.y) && (otherMax.x >= thisMin.x && otherMax.y >= thisMin.y);

        if(!check2)
        {
            return false;
        }

        return true;
    }
}
