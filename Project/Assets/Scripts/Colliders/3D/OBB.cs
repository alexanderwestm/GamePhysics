﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OBB : CollisionHull3D
{
    public Vector3 halfWidths;

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
        else if (renderer != null)
        {
            halfWidths.x = renderer.sprite.bounds.size.x * transform.localScale.x * .5f;
            halfWidths.y = renderer.sprite.bounds.size.y * transform.localScale.y * .5f;
            halfWidths.z = renderer.sprite.bounds.size.z * transform.localScale.z * .5f;
        }

        type = CollisionHullType3D.OBB;
    }

    public OBB(Vector3 halfWidths):base(CollisionHullType3D.OBB)
    {
        this.halfWidths = halfWidths;
    }

    protected override bool TestCollisionVsCircle(Sphere other, out Collision collision)
    {
        return CollisionHull3D.TestCollision(other, this, out collision);
    }

    protected override bool TestCollisionVsAABB(AABB other, out Collision collision)
    {
        return CollisionHull3D.TestCollision(other, this, out collision);
    }

    // TO DO
    protected override bool TestCollisionVsOBB(OBB other, out Collision collision)
    {
        collision = null;


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
        Vector3 thisMax, thisMin, otherMax, otherMin;
        Vector3 p1, p2, p3, p4;

        Vector3 otherPosition = other.particle.position;
        float thisLength = halfWidths[0];
        float thisHeight = halfWidths[1];
        float thisDepth = halfWidths[2];
        float otherLength = other.halfWidths[0];
        float otherHeight = other.halfWidths[1];
        float otherDepth = halfWidths[2];

        //get all corner points and then rotate it

        p1 = transform.worldToLocalMatrix * (new Vector3(thisLength, thisHeight));
        p2 = transform.worldToLocalMatrix * (new Vector3(thisLength, -thisHeight));
        p3 = transform.worldToLocalMatrix * (new Vector3(-thisLength, -thisHeight));
        p4 = transform.worldToLocalMatrix * (new Vector3(-thisLength, thisHeight));
        //for each corner, move it relative to the box then transform by the world matrix inverse. Finally add position back
        p1 = other.transform.worldToLocalMatrix * (p1 + particle.position - otherPosition);
        p2 = other.transform.worldToLocalMatrix * (p2 + particle.position - otherPosition);
        p3 = other.transform.worldToLocalMatrix * (p3 + particle.position - otherPosition);
        p4 = other.transform.worldToLocalMatrix * (p4 + particle.position - otherPosition);

        p1 += otherPosition;
        p2 += otherPosition;
        p3 += otherPosition;
        p4 += otherPosition;

        thisMax = new Vector3(Mathf.Max(p1.x, p2.x, p3.x, p4.x), Mathf.Max(p1.y, p2.y, p3.y, p4.y), Mathf.Max(p1.z, p2.z, p3.z, p4.z));
        thisMin = new Vector3(Mathf.Min(p1.x, p2.x, p3.x, p4.x), Mathf.Min(p1.y, p2.y, p3.y, p4.y), Mathf.Max(p1.z, p2.z, p3.z, p4.z));

        otherMax = new Vector3(otherPosition.x + otherLength, otherPosition.y + otherHeight);
        otherMin = new Vector3(otherPosition.x - otherLength, otherPosition.y - otherHeight);

        check1 = (thisMax.x >= otherMin.x && thisMax.y >= otherMin.y) && (otherMax.x >= thisMin.x && otherMax.y >= thisMin.y);

        if (!check1)
        {
            return false;
        }

        //get all corner points and then rotate it
        p1 = other.transform.worldToLocalMatrix * (new Vector3(otherLength, otherHeight));
        p2 = other.transform.worldToLocalMatrix * (new Vector3(otherLength, -otherHeight));
        p3 = other.transform.worldToLocalMatrix * (new Vector3(-otherLength, -otherHeight));
        p4 = other.transform.worldToLocalMatrix * (new Vector3(-otherLength, otherHeight));
        //for each corner, move it relative to the box then transform by the world matrix inverse. Finally add position back
        p1 = transform.worldToLocalMatrix * (p1 + otherPosition - particle.position); //this
        p2 = transform.worldToLocalMatrix * (p2 + otherPosition - particle.position);
        p3 = transform.worldToLocalMatrix * (p3 + otherPosition - particle.position);
        p4 = transform.worldToLocalMatrix * (p4 + otherPosition - particle.position);

        p1 += particle.position;
        p2 += particle.position;
        p3 += particle.position;
        p4 += particle.position;

        otherMax = new Vector3(Mathf.Max(p1.x, p2.x, p3.x, p4.x), Mathf.Max(p1.y, p2.y, p3.y, p4.y));
        otherMin = new Vector3(Mathf.Min(p1.x, p2.x, p3.x, p4.x), Mathf.Min(p1.y, p2.y, p3.y, p4.y));

        thisMax = new Vector3(particle.position.x + thisLength, particle.position.y + thisHeight);
        thisMin = new Vector3(particle.position.x - thisLength, particle.position.y - thisHeight);

        check2 = (thisMax.x >= otherMin.x && thisMax.y >= otherMin.y) && (otherMax.x >= thisMin.x && otherMax.y >= thisMin.y);

        if (!check2)
        {
            return false;
        }

        return true;
    }
}