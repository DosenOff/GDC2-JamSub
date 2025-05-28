using UnityEngine;

public class CellHealth : MonoBehaviour, IDamageable
{
    public int health = 100;
    public GameObject hitEffect;
    private SpriteRenderer spriteRenderer;

    public GameObject wallInstructions;

    void Start ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Instantiate(hitEffect, transform.position, Quaternion.identity);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, health / 100f);

        if (health <= 20)
        {
            Destroy(wallInstructions);
            Destroy(gameObject);
        }
    }
}
