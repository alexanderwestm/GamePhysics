using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTimer : MonoBehaviour
{
    private static ParticleTimer _instance;
    public static ParticleTimer Instance { get { return _instance; } }
    [SerializeField] float updatesPerSecond;
    [SerializeField] float secondsPerUpdate;
    public float timer;

    public bool update = false;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }

        timer = 0;

        secondsPerUpdate = 1 / updatesPerSecond;
    }

    private void Update()
    {
        update = secondsPerUpdate < timer;
        if(update)
        {
            timer = 0;
        }
        timer += Time.deltaTime;
    }
}
