﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    private PlayerStats player;
    public GameObject platform;
    private GameObject fish;
    private GameObject powerUp;
    private GameObject obstacle;

    private bool caughtFish = false;
    private bool caughtPowerUp = false;
    private bool caughtObstacle = false;

    private bool flag = false;
    private Vector2 endPoint;
    public float duration = 1f;
    public float speed = 4f;
    private Vector2 startPoint;
    private Transform originalPoint;
    RaycastHit2D hit;
    Vector2 ray;
    private float timeStartedLerping;
    bool hasArrived = true;

    void Start()
    {
        player = transform.parent.parent.parent.gameObject.GetComponent<PlayerStats>();
        speed = player.hookSpeed; // Get hookSpeed from PlayerStats.

        originalPoint = gameObject.transform;
        startPoint = gameObject.transform.localPosition;
    }

    void Update()
    {
        speed = player.hookSpeed; // Get hookSpeed from PlayerStats.

        ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast(ray, Vector2.zero);

        //check if the screen is touched / clicked   
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0)))
        {
            if (hasArrived)
                //Check if the ray hits any collider
                if (hit.collider != null)
                {
                    //set a flag to indicate to move the gameobject
                    flag = true;
                    timeStartedLerping = Time.time;
                    //save the click / tap position
                    endPoint = transform.InverseTransformPoint(hit.point);
                    endPoint.x = endPoint.x / 2;
                    endPoint.y = endPoint.y / 1.5f;
                    //Debug.Log(endPoint);
                }
        }

        if (flag)
        {
            hasArrived = false;
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted / duration;
            //Debug.Log(percentageComplete + "if");

            gameObject.transform.localPosition = Vector2.Lerp(gameObject.transform.localPosition, endPoint, percentageComplete * speed);

            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || (Input.GetMouseButtonUp(0)))
            {
                flag = false;
                timeStartedLerping = Time.time;
            }
        }
        else
        {
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted / duration;
            //Debug.Log(percentageComplete+"else");

            gameObject.transform.localPosition = Vector2.Lerp(gameObject.transform.localPosition, startPoint, percentageComplete * speed);

            if (Mathf.Approximately(Mathf.Round(gameObject.transform.localPosition.x),Mathf.Round(startPoint.x)) && Mathf.Approximately(Mathf.Round(gameObject.transform.localPosition.y), Mathf.Round(startPoint.y)))
                hasArrived = true;
        }

        if(caughtFish)
        {
            CatchFish();
        }
        if(caughtPowerUp)
        {
            CatchPowerUp();
        }
        if(caughtObstacle)
        {
            CatchObstacle();
        }
    }

    void CatchFish()
    {
        fish.transform.position = transform.position - new Vector3(10, 50, 0);

        if (transform.localPosition.y > -5)
        {
            caughtFish = false;

            float point = fish.GetComponent<SpawneeMovement>().point * player.scoreBoost;
            player.UpdateScore(point);
            platform.GetComponent<PlatformMelt>().IncreaseSize(fish.GetComponent<SpawneeMovement>().point / 7);

            float coin = fish.GetComponent<SpawneeMovement>().coin;
            player.UpdateCoins(coin);

            player.UpdateFishCount();
            Destroy(fish);
            GetComponent<BoxCollider2D>().enabled = true; // Enable to catch other spawnees.
        }
    }

    void CatchPowerUp()
    {
        powerUp.transform.position = transform.position - new Vector3(10, 50, 0);

        if(transform.localPosition.y > -5)
        {
            caughtPowerUp = false;

            float point = powerUp.GetComponent<SpawneeMovement>().point * player.scoreBoost;
            player.UpdateScore(point);
            platform.GetComponent<PlatformMelt>().IncreaseSize(1);

            float coin = powerUp.GetComponent<SpawneeMovement>().coin;
            player.UpdateCoins(coin);

            powerUp.GetComponent<PowerUp>().ApplyPowerUp(powerUp.name);

            GetComponent<BoxCollider2D>().enabled = true; // Enable to catch other spawnees.
        }
    }

    void CatchObstacle()
    {
        obstacle.transform.position = transform.position - new Vector3(10, 40, 0);

        if (transform.localPosition.y > -5)
        {
            caughtObstacle = false;

            GetComponent<BoxCollider2D>().enabled = true; // Enable to catch other spawnees.

            Destroy(obstacle);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Fish")
        {
            fish = other.gameObject;

            GetComponent<BoxCollider2D>().enabled = false; // Disable the collider so it wont collide with other fish or powerup.
                                                           // What if it's an enemy?

            if (fish.transform.parent.name == "LeftSpawn")
            {
                other.transform.Rotate(0, 0, 90);
            }
            else if (fish.transform.parent.name == "RightSpawn")
            {
                other.transform.Rotate(0, 0, -90);
            }

            caughtFish = true;
        }
        else if (other.tag == "PowerUp")
        {
            powerUp = other.gameObject;
            powerUp.GetComponent<SpawneeMovement>().enabled = false;

            GetComponent<BoxCollider2D>().enabled = false; // Disable the collider so it wont collide with other fish or powerup.
                                                           // What if it's an enemy?

            caughtPowerUp = true;
        }
        else if (other.tag == "Enemy")
        {
            GetComponent<BoxCollider2D>().enabled = false; // Disable the collider so it wont collide with other fish or powerup.
                                                           // What if it's an enemy?
                                                           //hookOn = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = false; // disable renderer so it will be invisible

            player.UpdateHealth(-1);
            StartCoroutine(waitHook()); // go on cooldown
        }
        else if (other.tag == "Obstacle")
        {
            obstacle = other.gameObject;

            GetComponent<BoxCollider2D>().enabled = false;
            caughtObstacle = true;
        }
    }

    IEnumerator waitHook()
    {
        yield return new WaitForSeconds(1.5f); // wait this seconds
        gameObject.GetComponent<SpriteRenderer>().enabled = true; // enable renderer
        GetComponent<BoxCollider2D>().enabled = true; // enable collider
       
    }
}

