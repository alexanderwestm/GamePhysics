using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSystem : MonoBehaviour
{
    List<CollisionHull2D> collisionHulls;
    List<CollisionHull2D.Collision> hullCollisions;
    private void Awake()
    {
        collisionHulls = new List<CollisionHull2D>(GameObject.FindObjectsOfType<CollisionHull2D>());
        hullCollisions = new List<CollisionHull2D.Collision>();
    }

    private void Update()
    {
        CheckCollisions();
        ResolveCollisions();
    }

    private void CheckCollisions()
    {
        CollisionHull2D.Collision collision = new CollisionHull2D.Collision();
        bool duplicate = false;
        for (int j = 0; j < collisionHulls.Count; ++j)
        {
            for (int k = j; k < collisionHulls.Count; ++k)
            {
                if(collisionHulls[j] != collisionHulls[k])
                {
                    if (CollisionHull2D.TestCollision(collisionHulls[j], collisionHulls[k], out collision))
                    {
                        foreach (CollisionHull2D.Collision col in hullCollisions)
                        {
                            if ((col.a == collision.a && col.b == collision.b) || (col.a == collision.b && col.b == collision.a))
                            {
                                duplicate = true;
                            }
                        }
                        if (!duplicate)
                        {
                            hullCollisions.Add(collision);
                            collision = new CollisionHull2D.Collision();
                        }
                    }
                }
            }
        }
    }

    private void ResolveCollisions()
    {
        foreach (CollisionHull2D.Collision collision in hullCollisions)
        {
            ResolveVelocity(collision);
            ResolveInterpenetration(collision);
        }
        hullCollisions.Clear();
    }

    private void ResolveVelocity(CollisionHull2D.Collision collision)
    {
        // we are either separating or stationary
        // no collision needs to be resolved
        if(collision.closingVelocity > 0)
        {
            return;
        }

        CollisionHull2D.Collision.Contact[] contacts = collision.contact;
        float totalInverseMass = collision.a.particle.massInv + collision.b.particle.massInv;
        // both objects have infinite mass
        if(totalInverseMass <= 0)
        {
            return;
        }

        for (int i = 0; i < collision.contactCount; ++i)
        {
            CollisionHull2D.Collision.Contact contact = contacts[i];
            float realSeparating = -collision.closingVelocity * contact.restitution;
            float deltaVelocity = realSeparating - collision.closingVelocity;
            float impulse = deltaVelocity / totalInverseMass;
            Vector2 impulsePerIMass = contact.normal * impulse;
            collision.a.particle.velocity += (impulsePerIMass * collision.a.particle.massInv);
            collision.b.particle.velocity += (impulsePerIMass * -collision.b.particle.massInv);
        }
    }

    private void ResolveInterpenetration(CollisionHull2D.Collision collision)
    {
        for (int i = 0; i < collision.contactCount; ++i)
        {
            CollisionHull2D.Collision.Contact contact = collision.contact[i];
            if (contact.penetrationDepth > 0)
            {
                float totalInvMass = collision.a.particle.massInv + collision.b.particle.massInv;
                if (totalInvMass > 0)
                {
                    Vector2 movePerIMass = contact.normal * (contact.penetrationDepth / totalInvMass);
                    collision.a.particle.position += movePerIMass * collision.a.particle.massInv;
                    collision.b.particle.position += movePerIMass * -collision.b.particle.massInv;
                }
            }
        }
    }
}
