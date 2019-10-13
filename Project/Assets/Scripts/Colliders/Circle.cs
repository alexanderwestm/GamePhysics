using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Circle : CollisionHull2D
{
    public float radius { get; private set; }
    public Circle(float r) : base(CollisionHullType2D.CIRCLE)
    {
        radius = r;
    }

    public void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if(meshFilter != null)
        {
            radius = meshFilter.mesh.bounds.size.x * transform.localScale.x * .5f;

        }
        else if(renderer != null)
        {

            radius = renderer.sprite.bounds.size.x * transform.localScale.x * .5f;
        }
        base.type = CollisionHullType2D.CIRCLE;
    }

    protected override bool TestCollisionVsCircle(Circle other, out Collision collision)
    {
        collision = null;
        Vector2 distance = particle.position - other.particle.position;

        float radiusSum = radius + other.radius;
        bool colliding = Vector3.Dot(distance, distance) <= radiusSum * radiusSum;

        if (colliding)
        {
            collision = new Collision();
            Vector2 relativeVelocity = particle.velocity - other.particle.velocity;
            // 7.1.1 closing velocity
            collision.closingVelocity = Vector3.Dot(relativeVelocity, distance.normalized);
            // assuming one point of contact
            collision.contact[0].normal = distance.normalized;
            collision.contact[0].point = radius * collision.contact[0].normal;
            collision.contact[0].restitution = .5f;
            collision.contact[0].penetrationDepth = radiusSum - distance.magnitude;
            collision.a = this;
            collision.b = other;
            collision.status = colliding;
            collision.contactCount = 1;
        }
        return colliding;
    }

    protected override bool TestCollisionVsAABB(AABB other, out Collision collision)
    {
        collision = null;
        // find closest point to the circle on the box: clamp the (center - min) and (center - max)
        // circle point collision test using closest point
        Vector2 point, otherMin, otherMax, delta;

        otherMin = other.particle.position - other.halfWidths;
        otherMax = other.particle.position + other.halfWidths;

        // find the closest point on the aabb
        point.x = Mathf.Clamp(particle.position.x, otherMin.x, otherMax.x);
        point.y = Mathf.Clamp(particle.position.y, otherMin.y, otherMax.y);

        delta = particle.position - point;
        bool colliding = delta.sqrMagnitude < radius * radius;

        if(colliding)
        {
            collision = new Collision();
            Vector2 relativeVelocity = particle.velocity - other.particle.velocity;
            Vector2 relativePosition = particle.position - other.particle.position;
            // 7.1.1 closing velocity
            collision.closingVelocity = Vector3.Dot(relativeVelocity, relativePosition.normalized);
            // assuming one point of contact
            collision.contact[0].normal = delta.normalized;
            collision.contact[0].point = point;
            collision.contact[0].restitution = .5f;
            collision.contact[0].penetrationDepth = radius - delta.magnitude;
            collision.a = this;
            collision.b = other;
            collision.status = colliding;
            collision.contactCount = 1;
        }

        // point collision with circle test
        return colliding;
    }

    protected override bool TestCollisionVsOBB(OBB other, out Collision collision)
    {
        collision = null;
        // same as aabb
        // multiply circle center by box world matrix inverse
        //other.transform.worldToLocalMatrix
        Matrix4x4 otherMat = other.transform.worldToLocalMatrix;
        Vector2 minExtents = other.particle.position - other.halfWidths, maxExtents = other.particle.position + other.halfWidths;
        Vector2 adjustedCenter = otherMat.MultiplyPoint3x4(new Vector3(particle.position.x, particle.position.y, 0));
        adjustedCenter.x *= other.transform.localScale.x;
        adjustedCenter.y *= other.transform.localScale.y;
        adjustedCenter += (Vector2)other.transform.position;
        //adjustedCenter += particle.position;

        Vector2 closestPoint;

        closestPoint.x = Mathf.Clamp(adjustedCenter.x, minExtents.x, maxExtents.x);
        closestPoint.y = Mathf.Clamp(adjustedCenter.y, minExtents.y, maxExtents.y);

        Vector2 deltaPos = adjustedCenter - closestPoint;

        bool colliding = deltaPos.sqrMagnitude < radius * radius;

        if(colliding)
        {
            collision = new Collision();
            Vector2 relativeVelocity = particle.velocity - other.particle.velocity;
            Vector2 relativePosition = particle.position - other.particle.position;
            // 7.1.1 closing velocity
            collision.closingVelocity = Vector3.Dot(relativeVelocity, relativePosition.normalized);
            // assuming one point of contact
            collision.contact[0].normal = deltaPos.normalized;
            collision.contact[0].point = closestPoint;
            collision.contact[0].restitution = .5f;
            collision.contact[0].penetrationDepth = radius - deltaPos.magnitude;
            collision.a = this;
            collision.b = other;
            collision.status = colliding;
            collision.contactCount = 1;
        }

        return colliding;
    }
}
