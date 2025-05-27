using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skateboard : MonoBehaviour
{
    private Player player;

    Rigidbody2D rb;
    Collider2D col;

    public bool cantDamage = false;
    [SerializeField] bool touched = false;
    bool isWaiting = false;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DetectSkateboard"))
        {
            if (player.currentState == Player.SkateboardState.Thrown)
            {
                if (touched)
                {
                    player.currentState = Player.SkateboardState.Idle;
                    player.holding = true;
                    touched = false;
                    cantDamage = false;
                }

                else
                    StartCoroutine(DelayPickUp());
            }

            else if (player.currentState != Player.SkateboardState.Charging)
            {
                player.currentState = Player.SkateboardState.Idle;
                player.holding = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("DetectSkateboard"))
        {
            if (player.currentState == Player.SkateboardState.Thrown)
            {
                if (touched)
                {
                    player.currentState = Player.SkateboardState.Idle;
                    player.holding = true;
                    touched = false;
                }

                else
                    StartCoroutine(DelayPickUp());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (cantDamage)
            return;

        IDamageable idamageable = collision.collider.GetComponent<IDamageable>();

        if (idamageable != null)
        {
            if (player.currentState == Player.SkateboardState.Thrown)
            {
                int damage = Mathf.RoundToInt(collision.relativeVelocity.magnitude * 2f);
                idamageable.TakeDamage(damage);
                Debug.Log("Speed is " + collision.relativeVelocity.magnitude + " Damage is " + damage);
            }

            else if (player.currentState != Player.SkateboardState.Charging)
            {
                int damage = Mathf.RoundToInt(collision.relativeVelocity.magnitude / 2f);
                idamageable.TakeDamage(damage);
                Debug.Log("Speed is " + collision.relativeVelocity.magnitude + " Damage is " + damage);
            }
        }

        if (player.currentState == Player.SkateboardState.Thrown)
            cantDamage = true;
    }

    IEnumerator DelayPickUp()
    {
        if (isWaiting)
            yield break;

        isWaiting = true;
        yield return new WaitForSeconds(0.25f);
        touched = true;
        isWaiting = false;
    }
}
