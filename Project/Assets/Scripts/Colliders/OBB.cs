using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBB : CollisionHull2D
{
    public OBB():base(CollisionHullType2D.OBB)
    {
        
    }

    protected override bool TestCollisionVsCircle(Circle other)
    {
        return false;
    }
    protected override bool TestCollisionVsAABB(AABB other)
    {
        return false;
    }
    protected override bool TestCollisionVsOBB(OBB other)
    {
        return false;
    }
}
