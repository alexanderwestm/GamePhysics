using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLaser : MonoBehaviour
{
    [SerializeField] GameObject laserPrefab;
    public int laserCount;

    private int allowedLasers = 3;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && laserCount < allowedLasers)
        {
            laserCount++;
            GameObject obj = Instantiate(laserPrefab, transform.position, transform.rotation);
            obj.GetComponent<LaserScript>().laserSpawnScript = this;
        }
    }
}
