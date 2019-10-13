using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour
{
    [SerializeField] float laserSpeed = 1;
    public SpawnLaser laserSpawnScript;
    private Camera camera;
    public void Start()
    {
        Particle2D particle = GetComponent<Particle2D>();
        particle.AddForce(transform.up * laserSpeed);
        CollisionSystem.Instance.AddCollisionHull(GetComponent<CollisionHull2D>());
        camera = Camera.main;
    }

    private void Update()
    {
        Vector2 point = camera.WorldToViewportPoint(transform.position);
        if(point.x > 1 || point.x < 0 || point.y > 1 || point.y < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        CollisionSystem.Instance.RemoveCollisionHull(GetComponent<CollisionHull2D>());
        laserSpawnScript.laserCount--;
    }
}
