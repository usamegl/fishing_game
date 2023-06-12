﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    private PlayerStats player;
    public GameObject platform;

    private float width = Screen.width;
    private float height = Screen.height;
    private float charWidth;
    private Vector2 pos;
    private Vector2 bottomCorner;
    private Vector2 topCorner;
    private float center;

    private void Awake()
    {
        player = transform.parent.GetComponentInChildren<PlayerStats>();
    }

    private void Start()
    {
        pos = this.transform.position;
        charWidth = platform.GetComponent<SpriteRenderer>().bounds.size.x;
        center = width / 2;
        float camDistance = Vector2.Distance(transform.position, Camera.main.transform.position);
        bottomCorner = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, camDistance));
        topCorner = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, camDistance));
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        float moveSpeed = player.moveSpeed;

        pos = this.transform.position;
       
        if ((Input.GetMouseButton(0) && Input.mousePosition.y > ((height*60)/100)) || (Input.touchCount > 0))
        {
#if UNITY_EDITOR
            if (pos.x > bottomCorner.x + charWidth / 2)
                if (Input.GetMouseButton(0) && Input.mousePosition.x < center - 0.5)
                {
                    pos.x = pos.x - moveSpeed;
                    this.transform.position = pos;
                }
            if (pos.x < topCorner.x - charWidth / 2)
                if (Input.GetMouseButton(0) && Input.mousePosition.x > center + 0.5)
                {
                    pos.x = pos.x + moveSpeed;
                    this.transform.position = pos;
                }

#endif
#if UNITY_ANDROID || UNITY_IOS
            int touchCount = Input.touchCount;
            for (int i = 0; i < touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.position.y > ((height * 60) / 100)) {
                    if (pos.x > bottomCorner.x + charWidth / 2)
                        if (touch.position.x < center - 0.5)
                        {
                            pos.x = pos.x - moveSpeed;
                            this.transform.position = pos;
                        }
                    if (pos.x < topCorner.x - charWidth / 2)
                        if (touch.position.x > center + 0.5)
                        {
                            pos.x = pos.x + moveSpeed;
                            this.transform.position = pos;
                        }
                }
            }
#endif
        }
    }
}
