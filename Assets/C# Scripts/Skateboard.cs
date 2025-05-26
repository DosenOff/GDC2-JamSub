using UnityEngine;

public class Skateboard : MonoBehaviour
{
    private Player player;

    Rigidbody2D rb;
    Collider2D col;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DetectSkateboard"))
        {
            if (player.currentState != Player.SkateboardState.Charging)
            {
                player.currentState = Player.SkateboardState.Idle;
                player.holding = true;
            }
        }
    }
}
