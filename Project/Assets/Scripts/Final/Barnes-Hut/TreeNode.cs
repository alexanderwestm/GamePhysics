using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuadrantDirection
{
    NorthWest = 0,
    NorthEast,
    SouthWest,
    SouthEast
}


public class TreeNode
{
    // our quadrant data
    public Quadrant quadrant;
    // unsure how to get this setup
    public float totalMass { get; private set; }
    public Vector3 centerOfMass;
    // our child quadrants
    Quadrant[] quadrants;

    TreeNode(Quadrant quad)
    {
        quadrant = quad;
        quadrants = new Quadrant[4];
    }

    public Quadrant GetChildQuadrant(QuadrantDirection direction)
    {
        return quadrants[(int)direction];
    }

    public bool IsExternal()
    {
        return quadrants[(int)QuadrantDirection.NorthWest] == null && quadrants[(int)QuadrantDirection.NorthEast] == null 
                && quadrants[(int)QuadrantDirection.SouthWest] == null && quadrants[(int)QuadrantDirection.SouthEast] == null;
    }
}
