﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;
    Particle2D playerParticle;

    [SerializeField] GameObject leftThruster;
    [SerializeField] GameObject rightThruster;

    private void Awake()
    {
        if(player == null)
        {
            player = GameObject.Find("Player");
        }
        playerParticle = player.GetComponent<Particle2D>();
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 moveVector = Vector2.zero;
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            playerParticle.AddForce(player.transform.up * speed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            //point = transform.worldToLocalMatrix.MultiplyPoint3x4(new Vector2(-.9f, -.9f));
            // this is wonky af still
            Vector2 point = playerParticle.centerOfMassLocal + new Vector2(-1f, -1f);
            playerParticle.AddTorque(Vector2.up * rotationSpeed, point * player.transform.localScale, true);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            //point = transform.worldToLocalMatrix.MultiplyPoint3x4(new Vector2(.9f, -.9f));
            // this is wonky af still
            Vector2 point = playerParticle.centerOfMassLocal + new Vector2(1f, -1f);
            playerParticle.AddTorque(Vector2.up * rotationSpeed, point * player.transform.localScale, true);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            playerParticle.AddForce(player.transform.up * -speed);
        }
    }
}