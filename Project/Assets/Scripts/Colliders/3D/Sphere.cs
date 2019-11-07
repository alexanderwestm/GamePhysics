using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sphere : CollisionHull3D
{
    public float radius { get; private set; }
    public Sphere(float r) : base(CollisionHullType3D.SPHERE)
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
        type = CollisionHullType3D.SPHERE;
    }

    protected override bool TestCollisionVsCircle(Sphere other, out Collision collision)
    {
        collision = null;
        Vector3 distance = particle.position - other.particle.position;

        float radiusSum = radius + other.radius;
        bool colliding = Vector3.Dot(distance, distance) <= radiusSum * radiusSum;

        //if (colliding)
        //{
        //    collision = new Collision();
        //    Vector3 relativeVelocity = particle.velocity - other.particle.velocity;
        //    // 7.1.1 closing velocity
        //    collision.closingVelocity = Vector3.Dot(relativeVelocity, distance.normalized);
        //    // assuming one point of contact
        //    collision.contact[0].normal = distance.normalized;
        //    collision.contact[0].point = radius * collision.contact[0].normal;
        //    collision.contact[0].restitution = .5f;
        //    collision.contact[0].penetrationDepth = radiusSum - distance.magnitude;
        //    collision.a = this;
        //    collision.b = other;
        //    collision.status = colliding;
        //    collision.contactCount = 1;
        //}
        return colliding;
    }

    protected override bool TestCollisionVsAABB(AABB other, out Collision collision)
    {
        collision = null;
        // find closest point to the circle on the box: clamp the (center - min) and (center - max)
        // circle point collision test using closest point
        Vector3 point = particle.position, otherMin, otherMax, delta;

        otherMin = other.particle.position - other.halfWidths;
        otherMax = other.particle.position + other.halfWidths;

        // find the closest point on the aabb
        point = point.Vector3Clamp(otherMin, otherMax);

        //point.x = Mathf.Clamp(particle.position.x, otherMin.x, otherMax.x);
        //point.y = Mathf.Clamp(particle.position.y, otherMin.y, otherMax.y);
        //point.z = Mathf.Clamp(particle.position.z, otherMin.z, otherMax.z);

        delta = particle.position - point;
        bool colliding = delta.sqrMagnitude < radius * radius;

        //if(colliding)
        //{
        //    collision = new Collision();
        //    Vector3 relativeVelocity = particle.velocity - other.particle.velocity;
        //    Vector3 relativePosition = particle.position - other.particle.position;
        //    // 7.1.1 closing velocity
        //    collision.closingVelocity = Vector3.Dot(relativeVelocity, relativePosition.normalized);
        //    // assuming one point of contact
        //    collision.contact[0].normal = delta.normalized;
        //    collision.contact[0].point = point;
        //    collision.contact[0].restitution = .5f;
        //    collision.contact[0].penetrationDepth = radius - delta.magnitude;
        //    collision.a = this;
        //    collision.b = other;
        //    collision.status = colliding;
        //    collision.contactCount = 1;
        //}

        // point collision with circle test
        return colliding;
    }

    Vector3 test;

    protected override bool TestCollisionVsOBB(OBB other, out Collision collision)
    {
        collision = null;
        // same as aabb
        // multiply circle center by box world matrix inverse
        //other.transform.worldToLocalMatrix
        // to fix
        Matrix4x4 otherMat = other.particle.worldTransformMatrixInverse;
        //Matrix4x4 otherMat = other.transform.worldToLocalMatrix;
        Vector3 minExtents = other.particle.position - other.halfWidths, maxExtents = other.particle.position + other.halfWidths;
        Vector3 adjustedCenter = otherMat.MultiplyPoint3x4(particle.position);
        //adjustedCenter.x *= other.transform.localScale.x;
        //adjustedCenter.y *= other.transform.localScale.y;
        adjustedCenter += other.transform.position;

        test = adjustedCenter;

        Vector3 closestPoint = adjustedCenter;

        closestPoint = closestPoint.Vector3Clamp(minExtents, maxExtents);

        Vector3 deltaPos = adjustedCenter - closestPoint;

        bool colliding = deltaPos.sqrMagnitude < radius * radius;

        //if(colliding)
        //{
        //    collision = new Collision();
        //    Vector2 relativeVelocity = particle.velocity - other.particle.velocity;
        //    Vector2 relativePosition = particle.position - other.particle.position;
        //    // 7.1.1 closing velocity
        //    collision.closingVelocity = Vector3.Dot(relativeVelocity, relativePosition.normalized);
        //    // assuming one point of contact
        //    collision.contact[0].normal = deltaPos.normalized;
        //    collision.contact[0].point = closestPoint;
        //    collision.contact[0].restitution = .5f;
        //    collision.contact[0].penetrationDepth = radius - deltaPos.magnitude;
        //    collision.a = this;
        //    collision.b = other;
        //    collision.status = colliding;
        //    collision.contactCount = 1;
        //}

        return colliding;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(test, .1f);
    }
}
