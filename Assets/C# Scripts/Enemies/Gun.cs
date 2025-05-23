using UnityEditor;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float shootCooldown = 1f;
    public float predictionTime = 0.5f;

    private Transform handle;
    Vector3 initialHandlePos;
    bool isFlipped = false;

    Player player;
    Guard guard;

    void OnDrawGizmos()
    {
        guard = transform.gameObject.GetComponentInParent<Guard>();
        Gizmos.DrawWireSphere(transform.position, guard.detectionRange);
    }

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        handle = transform.GetChild(0).GetComponent<Transform>();
        initialHandlePos = handle.localPosition;
    }

    void Update()
    {
        int layerMask = ~(1 << gameObject.layer);
        Vector2 direction = (Vector2)(player.transform.position - transform.position).normalized;
        RaycastHit2D lineOfSight = Physics2D.Raycast(transform.position, direction, guard.detectionRange, layerMask);

        if (lineOfSight.collider != null)
        {
            Debug.DrawRay(transform.position, direction * guard.detectionRange, lineOfSight.collider.CompareTag("Player") ? Color.red : Color.green);

            if (lineOfSight.collider.CompareTag("Player"))
            {
                Shoot();
                FlipGFX(direction);
            }
        }
    }

    void Shoot()
    {
        float angle = Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        InvokeRepeating("FireBullet", 0f, shootCooldown);
    }

    void FireBullet()
    {
        float angleDif = Prediction();
        Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    }

    float Prediction()
    {
        Vector2 playerPredictedLoc = (Vector2)player.transform.position + (player.rb.linearVelocity * predictionTime);

        return
    }

    void FlipGFX(Vector2 direction)
    {
        if (direction.x < 0f && !isFlipped)
        {
            handle.localPosition = new Vector3(handle.localPosition.x, -handle.localPosition.y, handle.localPosition.z);
            isFlipped = true;
        }
        else if (direction.x > 0f && isFlipped)
        {
            handle.localPosition = initialHandlePos;
            isFlipped = false;
        }
    }
}
