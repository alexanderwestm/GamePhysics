using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleWrap : MonoBehaviour
{
    Particle2D particle;
    Camera camera;

    bool wrappingX, wrappingY;
    private void Start()
    {
        particle = GetComponent<Particle2D>();
        camera = Camera.main;
    }
    private void Update()
    {

        Vector2 position = camera.WorldToViewportPoint(particle.position);
        bool xOutOfBounds = (position.x > 1 || position.x < 0), yOutOfBounds = (position.y > 1 || position.y < 0);

        if (wrappingX && !xOutOfBounds)
        {
            wrappingX = false;
        }
        else if (!wrappingX && xOutOfBounds)
        {
            particle.position.x = -particle.position.x;
            wrappingX = true;
        }
        if (wrappingY && !yOutOfBounds)
        {
            wrappingY = false;
        }
        else if (!wrappingY && yOutOfBounds)
        {
            particle.position.y = -particle.position.y;
            wrappingY = true;
        }
    }
}
