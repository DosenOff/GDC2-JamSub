using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform playerTransform;
    private Player player;

    public bool isPanning = true;

    private float playerX;
    private float playerY;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void LateUpdate()
    {
        if (PlayerPrefs.HasKey("IsPanning"))
        {
            isPanning = PlayerPrefs.GetInt("IsPanning") != 0;
        }

        Vector3 temp = transform.position;

        if (!player.dead)
        {
            playerX = playerTransform.position.x;
            playerY = playerTransform.position.y;
        }

        float camX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x / 25f;
        float camY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y / 25f;

        if (isPanning)
        {
            if (playerX + camX > playerX + 2f)
            {
                temp.x = playerX + 2f;
            }

            else if (playerX + camX < playerX - 2f)
            {
                temp.x = playerX - 2f;
            }

            else
            {
                temp.x = playerX + camX;
            }

            if (playerY + camY > playerY + 2f)
            {
                temp.y = playerY + 2f;
            }

            else if (playerY + camY < playerY - 2f)
            {
                temp.y = playerY - 2f;
            }

            else
            {
                temp.y = playerY + camY;
            }
        }

        if (!isPanning)
        {
            temp.x = playerX;
            temp.y = playerY;
        }

        transform.position = temp;
    }
}
