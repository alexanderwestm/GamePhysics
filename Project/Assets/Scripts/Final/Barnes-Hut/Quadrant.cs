using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quadrant
{
    public Vector3 center { get; private set; }
    // half widths of the quadrant
    public Vector3 halfWidths { get; private set; }
    // if this is null it is an external node
    // if this is not null it is an internal node
    // the internal nodes are quadrants of just the particle
    public NParticle particle { get; private set; }

    Quadrant(Vector3 center, Vector3 halfWidths, NParticle particle = null)
    {
        this.center = center;
        this.halfWidths = halfWidths;
        this.particle = particle;
    }

    public Quadrant[] DivideQuadrant()
    {
        Quadrant[] quadrants = new Quadrant[4];
        quadrants[(int)QuadrantDirection.NorthWest] = new Quadrant(center + new Vector3(-halfWidths.x, halfWidths.y, 0) * .5f, halfWidths * .5f);
        quadrants[(int)QuadrantDirection.NorthEast] = new Quadrant(center + new Vector3(halfWidths.x, halfWidths.y, 0) * .5f, halfWidths * .5f);
        quadrants[(int)QuadrantDirection.SouthWest] = new Quadrant(center + new Vector3(-halfWidths.x, -halfWidths.y, 0) * .5f, halfWidths * .5f);
        quadrants[(int)QuadrantDirection.SouthEast] = new Quadrant(center + new Vector3(halfWidths.x, -halfWidths.y, 0) * .5f, halfWidths * .5f);

        return quadrants;
    }

    public bool IsPointIn(Vector3 point)
    {
        Vector3 min = center - halfWidths, max = center + halfWidths;
        return point.x >= min.x && point.y >= min.y && point.z >= min.z
                && point.x <= max.x && point.y <= max.y && point.z <= min.z;
    }
}
