﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitRangerController : MonoBehaviour {

    private List<GameObject> left;     // Holds enemies on the left
    private List<GameObject> right;    // Holds enemies on the right

    public GameObject player;
    public GameObject text;

    private PlayerController playerController;
    private TextMesh scoreText;
    private Animator anim;

    public int score = 0;
    private int multi;

    // Use this for initialization
    void Start ()
    {
        left = new List<GameObject>();
        right = new List<GameObject>();

        // Get the player controller
        playerController = player.GetComponent<PlayerController>();
        anim = player.GetComponentInChildren<Animator>();
        scoreText = text.GetComponent<TextMesh>();
        score = 0;
    }

    // As enemies enter range, list them for the correct side
    private void OnTriggerEnter(Collider other)
    {
        GameObject enemy = other.gameObject;
        if (enemy.tag == "Enemy")
        {
            if (enemy.transform.position.x < 0)
            {
                left.Add(enemy);
            }
            else if (enemy.transform.position.x > 0)
            {
                right.Add(enemy);
            }
        }
    }

    // Hits the clossest enemy on the chosen side
    public void HitEnemy(bool hitLeft)
    {
        // Check for super power
        if (playerController.powerUp != null)
        {
            // Get controller of players hit ranger
            PowerController powerController = playerController.powerUp.GetComponent<PowerController>();
            powerController.UsePower(hitLeft);
            playerController.powerUp = null;
            score += multi * 5;
            scoreText.text = score.ToString();
            return;
        }

        // Play correct animation
        if (hitLeft)
            anim.SetTrigger("Left");
        else
            anim.SetTrigger("Right");

        // Get correct enemies
        List<GameObject> list = right;
        if (hitLeft)
        {
            list = left;
        }

        // If no enemy leave function
        if (list.Count == 0)
        {
            multi = 1;
            return;
        }
        GameObject enemy = list[0];

        // Error check
        if (enemy == null)
            return;

        // Get controller of enemy
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        enemyController.GetHit();
        // Hit enemy and  remove if hp is zero
        if (enemyController.currentHP <= 0)
        {
            int temp = multi;
            multi = score;
            score += temp;
            list.Remove(enemy);
            scoreText.text = score.ToString();
               
            if (multi == 0)
            {
                multi = 1;
            }
        }
    }

    public void HitPlayer(EnemyController enemy)
    {
        multi = 1;

        playerController.GetHit();

        if (enemy.transform.position.x < 0)
        {
            left.Remove(enemy.gameObject);
        }
        else if (enemy.transform.position.x > 0)
        {
            right.Remove(enemy.gameObject);
        }
    }
}
