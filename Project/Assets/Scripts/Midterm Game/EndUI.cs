using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    private void Awake()
    {
        text.text = "Final Score: " + AsteroidGameManager.Instance.score;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
