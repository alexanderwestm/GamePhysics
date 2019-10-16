using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidScript : MonoBehaviour
{
    enum AsteroidType
    {
        NONE = -1,
        SMALL,
        MEDIUM,
        LARGE
    }
    [SerializeField] AsteroidType type;
    [SerializeField] GameObject smallAsteroid;
    [SerializeField] List<GameObject> mediumAsteroids;

    private float upperBoundSpeed = 250;
    private float lowerBoundSpeed = 100;
    Vector3 directionToPlayer;

    private void Awake()
    {
        Vector2 direction = new Vector2(Mathf.Cos(transform.rotation.z), Mathf.Sin(transform.rotation.z));
        float speed = Random.Range(lowerBoundSpeed, upperBoundSpeed);
        GetComponent<Particle2D>().AddForce(direction * speed);
        CollisionSystem.Instance.AddCollisionHull(GetComponent<CollisionHull2D>());
    }

    public void Remove()
    {
        switch(type)
        {
            case AsteroidType.SMALL:
            {
                // award a large amount of points
                AsteroidGameManager.Instance.ChangeScore(1000);
                SpawnAsteroids.Instance.numAsteroids--;
                break;
            }
            case AsteroidType.MEDIUM:
            {
                // award a medium amount of points
                // split into 3 small ones
                AsteroidGameManager.Instance.ChangeScore(500);
                for(int i = 0; i < 3; ++i)
                {
                    Instantiate(smallAsteroid, transform.position, Random.rotation);
                    SpawnAsteroids.Instance.numAsteroids++;
                }
                SpawnAsteroids.Instance.numAsteroids--;
                break;
            }
            case AsteroidType.LARGE:
            {
                // award a small amount of points
                // split into 2 medium ones
                AsteroidGameManager.Instance.ChangeScore(250);
                for (int i = 0; i < 2; ++i)
                {
                    int randomNum = Random.Range(0, 2);
                    Instantiate(mediumAsteroids[randomNum], transform.position, Random.rotation);
                    SpawnAsteroids.Instance.numAsteroids++;
                }
                SpawnAsteroids.Instance.numAsteroids--;
                break;
            }
        }
    }
}
