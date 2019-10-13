using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AsteroidGameManager : MonoBehaviour
{
    private static AsteroidGameManager _instance;
    public static AsteroidGameManager Instance { get { return _instance; } }

    private float score;
    private float playerHealth = 100;
    [SerializeField] float playerLives;
    private bool playerDead;

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Slider healthBar;
    [SerializeField] Sprite fullHeart;
    [SerializeField] Sprite emptyHeart;
    [SerializeField] List<Image> heartImages;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        score = 0;
        playerHealth = 0;
    }
    private void Update()
    {
        healthBar.value = playerHealth;
        if(!playerDead)
        {
            scoreText.text = "Score: " + score;
        }
        else if(playerLives >= 0)
        {
            playerHealth = 100;
            GameObject.Find("Player").GetComponent<Particle2D>().ReInit();
            int i = 0;
            while (i < playerLives)
            {
                heartImages[i].sprite = fullHeart;
                ++i;
            }
            while (i < heartImages.Count)
            {
                heartImages[i].sprite = emptyHeart;
                --i;
            }
        }
    }

    private void FixedUpdate()
    {
        ChangeScore(1f);
    }

    public void ChangeScore(float num)
    {
        score += num;
    }

    public void ChangePlayerHealth(float separatingVelocity)
    {
        playerHealth += separatingVelocity;
        if(playerHealth <= 0)
        {
            playerLives--;
        }
    }
}
