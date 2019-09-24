using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABB
{
    private bool TestCollisionVsCircle(Circle other)
    {
        // see circle
        return false;
    }

    private bool TestCollisionVsAABB(AABB other)
    {
        // if for all axes, max entent of A is greater than mix extent of B
        // 1. 
        return false;
    }

    private bool TestCollisionVsOBB(OBB other)
    {
        // same as above twice
        // find max extents of OBB, do ABB vs this box
        // then, transform this box into OBB's space, find max extents, repeat
        return false;
    }
}
