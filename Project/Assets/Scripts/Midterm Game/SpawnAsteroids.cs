using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAsteroids : MonoBehaviour
{
    private static SpawnAsteroids _instance { get; set; }
    public static SpawnAsteroids Instance { get { return _instance; } }
    [SerializeField] List<Transform> spawnLocations;
    [HideInInspector] public int numAsteroids;
    [SerializeField] int numAllowedAsteroids;
    [SerializeField] List<GameObject> asteroidPrefabs;

    float timer;
    [SerializeField] float spawnTime;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= spawnTime)
        {
            timer = 0;
            numAsteroids++;
            if (numAsteroids < numAllowedAsteroids)
            {
                int randNum = Random.Range(0, 101);
                int randLocation = Random.Range(0, spawnLocations.Count);
                if (randNum <= 50)
                {
                    randNum = 0;
                }
                else if(randNum <= 80)
                {
                    randNum = Random.Range(1, 3);
                }
                else
                {
                    randNum = 3;
                }
                Instantiate(asteroidPrefabs[randNum], spawnLocations[randLocation].position, Random.rotation);
            }
        }
    }
}
