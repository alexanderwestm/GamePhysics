using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NParticle))]
public class NSphereCollider : MonoBehaviour
{
    public NParticle particle { get; private set; }
    public float radius;

    private void Awake()
    {
        particle = GetComponent<NParticle>();
        radius = GetComponent<MeshRenderer>().bounds.size.x * .5f;
    }

    public bool CheckCollision(NSphereCollider other)
    {
        Vector3 difference = particle.position - other.particle.position;
        float sumRadii = radius + other.radius;
        float distanceSq = Vector3.Dot(difference, difference);
        return distanceSq <= sumRadii * sumRadii;
    }

    private void OnDestroy()
    {
        NCollisionChecker.Instance.RemoveItem(this);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
